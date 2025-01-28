using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalUser
{
    public int Id { get; set; }

    public EmailAddress Email { get; set; } = null!;

    public string Cid { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Nick { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Description { get; set; }

    public EmailAddress? RecoveryEmail { get; set; }

    public List<EmailAddress> Aliases { get; set; } = null!;
}