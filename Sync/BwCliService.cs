using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SyncIT.Sync;

public partial class BwCliService : IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly string _vaultwardenUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _password;
    private string? _sessionKey;
    private bool _disposed;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public BwCliService(ILoggerFactory loggerFactory,
        string vaultwardenUrl, string clientId, string clientSecret, string password)
    {
        _logger = loggerFactory.CreateLogger<BwCliService>();
        _vaultwardenUrl = vaultwardenUrl;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _password = password;
    }

    public async Task LoginAndUnlockAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Logging in");

        // Clear any lingering session so config switch works
        try { await RunBwAsync(["logout"], ct); } catch { /* ignore if not logged in */ }

        await RunBwAsync(["config", "server", _vaultwardenUrl], ct);
        await RunBwAsync(["login", "--apikey"], ct,
            ("BW_CLIENTID", _clientId), ("BW_CLIENTSECRET", _clientSecret));

        var output = await RunBwAsync(["unlock", "--passwordenv", "BW_PASSWORD"], ct,
            ("BW_PASSWORD", _password));

        _sessionKey = ExtractSessionKey(output);
        _logger.LogDebug("Logged in and unlocked successfully");
    }

    public async Task<List<BwMember>> ListOrgMembersAsync(string organizationId, CancellationToken ct = default)
    {
        EnsureSession();

        var output = await RunBwAsync(
            ["list", "--session", _sessionKey!, "--organizationid", organizationId, "org-members"],
            ct);

        return JsonSerializer.Deserialize<List<BwMember>>(output, JsonOptions) ?? [];
    }

    public async Task ConfirmMemberAsync(string organizationId, string userId, CancellationToken ct = default)
    {
        EnsureSession();

        await RunBwAsync(
            ["confirm", "--session", _sessionKey!, "org-member", userId, "--organizationid", organizationId],
            ct);
    }

    public async Task LockAsync()
    {
        if (_sessionKey == null) return;

        try
        {
            await RunBwAsync(["lock", "--session", _sessionKey], CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error locking vault (non-fatal)");
        }

        _sessionKey = null;
    }

    private void EnsureSession()
    {
        if (_sessionKey == null)
            throw new InvalidOperationException("Not logged in. Call LoginAndUnlockAsync first.");
    }

    private async Task<string> RunBwAsync(string[] args, CancellationToken ct,
        params (string key, string value)[] envVars)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "bw",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        // Inherit full parent environment, then add overrides
        var env = process.StartInfo.Environment;
        foreach (var key in Environment.GetEnvironmentVariables().Keys.Cast<string>())
        {
            if (!env.ContainsKey(key))
                env[key] = Environment.GetEnvironmentVariable(key) ?? "";
        }
        foreach (var (key, value) in envVars)
            env[key] = value;

        foreach (var arg in args)
            process.StartInfo.ArgumentList.Add(arg);

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync(ct);
        var error = await process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            var joined = string.Join(" ", args);
            throw new InvalidOperationException(
                $"bw {joined} failed (exit {process.ExitCode}): {error}");
        }

        return output;
    }

    private static string ExtractSessionKey(string output)
    {
        var match = SessionKeyRegex().Match(output);
        if (!match.Success)
            throw new InvalidOperationException($"Could not parse session key from bw output: {output}");
        return match.Groups[1].Value;
    }

    [GeneratedRegex(@"BW_SESSION=""([^""]+)""")]
    private static partial Regex SessionKeyRegex();

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await LockAsync();
        GC.SuppressFinalize(this);
    }
}
