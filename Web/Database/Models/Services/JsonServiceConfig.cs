namespace SyncIT.Web.Database.Models.Services;

public class JsonServiceConfig : BaseSyncServiceConfig
{
    public bool IsReadOnly { get; set; } = true;
    public string? FilePath { get; set; } = null!;
    
    public override bool CanBeTarget => !IsReadOnly;
}