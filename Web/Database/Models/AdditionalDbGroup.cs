using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalDbGroup
{
    public int Id { get; set; }
    public EmailAddress Email { get; set; } = null!;

    public string Name { get; set; } = null!;
    public bool OverwriteName { get; set; }

    public string? Description { get; set; }

    public List<EmailAddress> Members { get; set; } = null!;
    public List<EmailAddress> Aliases { get; set; } = null!;

    public AdditionalGroup ToAdditionalGroup()
    {
        return new AdditionalGroup(
            new Group(Email, new HashSet<EmailAddress>(Aliases), Name, new HashSet<EmailAddress>(Members)),
            OverwriteName);
    }
}