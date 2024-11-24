using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models.Services;

public class GammaServiceConfig : BaseSyncServiceConfig
{
    [RegularExpression(@"^https?:\/\/[a-zA-Z0-9.-]+(\.[a-zA-Z0-9.-]+)+$")]
    public string Url { get; set; } = null!;
    [MinLength(10)]
    public string Token { get; set; } = null!;
    [RegularExpression(@"[a-zA-Z0-9.-]+(\.[a-zA-Z0-9.-]+)+$")]
    public string EmailDomain { get; set; } = null!;
    
    public override bool CanBeTarget => false;
}