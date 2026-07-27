using SyncIT.Sync;

namespace SyncIT.Web.Database.Models;

public class BitwardenInstance
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string UrlBase { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;

    public DateTime? LastSync { get; set; }

    public int? LastUserCount { get; set; }
    public int? LastGroupCount { get; set; }

    public string? OrganizationId { get; set; }
    public string? BotClientId { get; set; }
    public string? BotClientSecret { get; set; }
    public string? BotPassword { get; set; }
    public DateTime? LastConfirmDate { get; set; }
    public int? LastConfirmCount { get; set; }

    public BitwardenSync.Credentials ToCredentials()
    {
        return new BitwardenSync.Credentials(ClientId, ClientSecret, UrlBase);
    }
}
