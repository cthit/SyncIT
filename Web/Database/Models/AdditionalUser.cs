using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalUser
{
    public int Id { get; set; }

    public EmailAddress Email { get; set; } = null!;

    public EmailAddress? RecoveryEmail { get; set; }
    public bool OverwriteRecoveryEmail { get; set; }

    public string Cid { get; set; } = null!;
    public bool OverwriteCid { get; set; }

    public string FirstName { get; set; } = null!;
    public bool OverwriteFirstName { get; set; }

    public string Nick { get; set; } = null!;
    public bool OverwriteNick { get; set; }

    public string LastName { get; set; } = null!;
    public bool OverwriteLastName { get; set; }

    public string? Description { get; set; }

    public List<EmailAddress> Aliases { get; set; } = null!;
}