using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SyncIT.Sync.Models;
using SyncIT.Sync.Services.Gamma;
using SyncIT.Sync.Utils;

namespace SyncIT.Sync;

public class BitwardenSync
{
    private readonly HttpClient _httpClient;

    public BitwardenSync(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Pushes users and groups from Gamma to Bitwarden
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="credentials"></param>
    /// <param name="expectedCounts"></param>
    /// <param name="dryRun"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<AffectedCounts> PushToBitwarden(GammaAccountServiceSettings settings, Credentials credentials,
        AffectedCounts? expectedCounts, bool dryRun = false)
    {
        var gammaAccountScaffoldApi = new GammaAccountScaffoldApi(_httpClient, settings.BaseUrl, settings.ApiKey);

        var gammaUsers = await gammaAccountScaffoldApi.GetUsersAsync().ConfigureAwait(false);
        var gammaGroups = await gammaAccountScaffoldApi.GetSuperGroupsAsync().ConfigureAwait(false);


        var bitwardenUsers = gammaUsers.Where(u => u.Cid == "gustafas").Select(gammaUser => new BitwardenUser(
            CreateSanitizedEmail(gammaUser.Cid, settings.AccountEmailDomain),
            gammaUser.Cid,
            false
        )).ToList();

        //We might want to have a better filer but this works for now
        var bitwardenGroups = gammaGroups.Where(gammaGroup => gammaGroup.Type != "alumni")
            .Select(gammaGroup => new BitwardenGroup(
                gammaGroup.PrettyName,
                gammaGroup.Name,
                gammaGroup.Groups.SelectMany(g => g.Members).Where(u => u.User.Cid == "gustafas")
                    .Select(m => m.User.Cid).ToHashSet()
            )).ToList();

        var affectedCounts = new AffectedCounts(bitwardenUsers.Count, bitwardenGroups.Count);

        if (dryRun)
            return affectedCounts;

        if (expectedCounts != null)
            if (expectedCounts.Users != affectedCounts.Users ||
                expectedCounts.Groups != affectedCounts.Groups)
                throw new Exception(
                    $"Expected counts do not match actual counts. Expected Users: {expectedCounts.Users}, Actual Users: {affectedCounts.Users}. Expected Groups: {expectedCounts.Groups}, Actual Groups: {affectedCounts.Groups}");

        var importData = new BitwardenOrgImportData(bitwardenUsers, bitwardenGroups, true, true);

        var json = JsonSerializer.Serialize(importData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var tokenRequest =
            new HttpRequestMessage(HttpMethod.Post, $"{credentials.UrlBase.TrimEnd('/')}/identity/connect/token")
            {
                Content = new StringContent(
                    $"grant_type=client_credentials&scope=api.organization&client_id={credentials.ClientId}&client_secret={credentials.ClientSecret}&device_identifier={Dns.GetHostName()}&device_name=SyncIT&device_type=server",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
            };

        var tokenResponse = await _httpClient.SendAsync(tokenRequest);
        tokenResponse.EnsureSuccessStatusCode();
        var tokenResponseBody = await tokenResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(tokenResponseBody).RootElement.GetProperty("access_token").GetString();
        if (string.IsNullOrEmpty(token))
            throw new Exception("Failed to get access token from Bitwarden");

        var request =
            new HttpRequestMessage(HttpMethod.Post,
                $"{credentials.UrlBase.TrimEnd('/')}/api/public/organization/import")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var result = await _httpClient.SendAsync(request);

        if (!result.IsSuccessStatusCode)
            throw new Exception(
                $"Bitwarden import failed with status code {result.StatusCode} and message: {await result.Content.ReadAsStringAsync()}");

        return affectedCounts;
    }

    private EmailAddress CreateSanitizedEmail(string localPart, string domain)
    {
        return new EmailAddress($"{EmailSanitizer.SanitizeLocal(localPart)}@{domain}");
    }

    public record AffectedCounts(int Users, int Groups);

    public record Credentials(string ClientId, string ClientSecret, string UrlBase);

    private record BitwardenUser(EmailAddress Email, string ExternalId, bool Deleted);

    private record BitwardenGroup(string Name, string ExternalId, HashSet<string> MemberExternalIds);

    private record BitwardenOrgImportData(
        List<BitwardenUser> Members,
        List<BitwardenGroup> Groups,
        bool OverwriteExisting,
        bool LargeImport);
}