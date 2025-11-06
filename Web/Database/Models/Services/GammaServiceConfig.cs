using SyncIT.Sync.Services;
using SyncIT.Sync.Services.Gamma;

namespace SyncIT.Web.Database.Models.Services;

public class GammaServiceConfig : BaseSyncServiceConfig
{
    public string Url { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string EmailDomain { get; set; } = null!;

    public override bool CanBeTarget => false;

    public GammaAccountService ToService(HttpClient client)
    {
        return new GammaAccountService(ToGammaSettings(), client);
    }

    public GammaAccountServiceSettings ToGammaSettings()
    {
        return new GammaAccountServiceSettings(Url, Token, EmailDomain);
    }

    public override ISource ToSource(IServiceProvider serviceProvider)
    {
        return ToService(serviceProvider.GetRequiredService<HttpClient>());
    }
}