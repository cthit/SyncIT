using System.Text.Json;

namespace SyncIT.Sync;

public class BwServeClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public BwServeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BwMember>> ListOrgMembersAsync(string organizationId, CancellationToken ct = default)
    {
        var response = await PostCommandAsync(new
        {
            command = "list",
            args = new { @object = "org-members", organizationid = organizationId }
        }, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"bw list failed: {response.StatusCode} - {content}");

        // bw serve returns 200 even on CLI errors; check for error response
        if (content.Contains("\"success\":false", StringComparison.OrdinalIgnoreCase))
        {
            var error = JsonSerializer.Deserialize<BwErrorResponse>(content, JsonOptions);
            throw new InvalidOperationException($"bw list failed: {error?.Message ?? "Unknown error"}");
        }

        return JsonSerializer.Deserialize<List<BwMember>>(content, JsonOptions) ?? [];
    }

    public async Task ConfirmMemberAsync(string organizationId, string userId, CancellationToken ct = default)
    {
        var response = await PostCommandAsync(new
        {
            command = "confirm",
            args = new { organizationid = organizationId, id = userId }
        }, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"bw confirm failed: {response.StatusCode} - {content}");

        if (content.Contains("\"success\":false", StringComparison.OrdinalIgnoreCase))
        {
            var error = JsonSerializer.Deserialize<BwErrorResponse>(content, JsonOptions);
            throw new InvalidOperationException($"bw confirm failed: {error?.Message ?? "Unknown error"}");
        }
    }

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/status");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<HttpResponseMessage> PostCommandAsync(object body, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/", content, ct);
        return response;
    }

    private record BwErrorResponse(bool Success, string? Message);
}
