using System.Text.Json;

namespace SyncIT.Sync.Services.Gamma;

public class GammaAccountScaffoldApi
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public GammaAccountScaffoldApi(HttpClient httpClient, string baseUrl, string apiKey)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    public async Task<List<GammaUser>> GetUsersAsync()
    {
        var response = await GammaRequest("api/account-scaffold/v1/users").ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<GammaUser>>(response, JsonSerializerOptions.Web) ??
               throw new InvalidDataException("Invalid JSON data");
    }

    public async Task<List<GammaSuperGroup>> GetSuperGroupsAsync()
    {
        var response = await GammaRequest("api/account-scaffold/v1/supergroups").ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<GammaSuperGroup>>(response, JsonSerializerOptions.Web) ??
               throw new InvalidDataException("Invalid JSON data");
    }

    private async Task<string> GammaRequest(string endpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{endpoint}");
        request.Headers.Add("Authorization", $"pre-shared {_apiKey}");
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}