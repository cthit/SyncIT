using SyncIT.Sync.Services;
using SyncIT.Sync.Services.Json;

namespace SyncIT.Web.Database.Models.Services;

public class JsonServiceConfig : BaseSyncServiceConfig
{
    public bool IsReadOnly { get; set; } = true;
    public string FilePath { get; set; } = null!;

    public override bool CanBeTarget => !IsReadOnly;

    public JsonAccountService ToService()
    {
        return new JsonAccountService(FilePath);
    }

    public override ISource ToSource(IServiceProvider _)
    {
        return ToService();
    }
}