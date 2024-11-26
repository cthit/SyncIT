using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models.Services;

public class GammaServiceConfig : BaseSyncServiceConfig
{
    public string Url { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string EmailDomain { get; set; } = null!;

    public override bool CanBeTarget => false;
}