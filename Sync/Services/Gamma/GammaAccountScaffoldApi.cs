using System.Text.Json;

namespace SyncIT.Sync.Services.Gamma;

public class GammaAccountScaffoldApi
{
    private HttpClient _httpClient;
    private string _baseUrl;
    private string _apiKey;
    
    public GammaAccountScaffoldApi(HttpClient httpClient, string baseUrl, string apiKey)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    public async Task<List<GammaUser>> GetUsersAsync()
    {
        var response = await GammaRequest("/api/account-scaffold/v1/users");
        return JsonSerializer.Deserialize<List<GammaUser>>(response) ?? throw new InvalidDataException("Invalid JSON data");
    }
    
    public async Task<List<GammaSuperGroup>> GetSuperGroupsAsync()
    {
        var response = await GammaRequest("/api/account-scaffold/v1/supergroups");
        return JsonSerializer.Deserialize<List<GammaSuperGroup>>(response) ?? throw new InvalidDataException("Invalid JSON data");
    }
    
    private async Task<string> GammaRequest(string endpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{endpoint}");
        request.Headers.Add("Authorization", $"pre-shared {_apiKey}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}