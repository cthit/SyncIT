using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services.Gamma;

/// <summary>
/// Account source from Gamma v2 also known as "Auth"
/// </summary>
public class GammaAccountService : IProvider
{
    private readonly GammaAccountScaffoldApi _gammaAccountScaffoldApi;
    private readonly GammaAccountServiceSettings _settings;
    
    public GammaAccountService(GammaAccountServiceSettings settings, HttpClient httpClient)
    {
        _gammaAccountScaffoldApi = new GammaAccountScaffoldApi(httpClient, settings.BaseUrl, settings.ApiKey);
        _settings = settings;
    }
    
    public async Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync()
    {
        var gammaUsers = await _gammaAccountScaffoldApi.GetUsersAsync();
        
        return gammaUsers.Select(gammaUser => new User(
            gammaUser.Cid,
            gammaUser.FirstName,
            gammaUser.LastName,
            gammaUser.Nick,
            $"{gammaUser.Cid}@{_settings.AccountEmailDomain}",
            new HashSet<EmailAddress>(){$"{gammaUser.Nick}@{_settings.AccountEmailDomain}"}
            )).ToDictionary(user => user.Email);
        
    }

    public Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync()
    {
        throw new NotImplementedException();
    }
}