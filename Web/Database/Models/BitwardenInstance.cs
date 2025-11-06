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

    public BitwardenSync.Credentials ToCredentials()
    {
        return new BitwardenSync.Credentials(ClientId, ClientSecret, UrlBase);
    }
}