using SyncIT.Sync.Services;
using SyncIT.Sync.Services.GSuite;

namespace SyncIT.Web.Database.Models.Services;

public class GSuiteServiceConfig : BaseSyncServiceConfig
{
    public string AuthJson { get; set; } = null!;

    public string AdminEmail { get; set; } = null!;

    public bool IsReadOnly { get; set; } = true;

    public override bool CanBeTarget => !IsReadOnly;

    public GSuiteAccountService ToService(ILogger<GSuiteAccountService> logger)
    {
        return new GSuiteAccountService(AuthJson, AdminEmail, logger);
    }

    public override ISource ToSource(IServiceProvider serviceProvider)
    {
        return ToService(serviceProvider.GetRequiredService<ILogger<GSuiteAccountService>>());
    }
}