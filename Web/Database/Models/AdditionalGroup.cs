using System.ComponentModel.DataAnnotations;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalGroup
{
    [Key] public EmailAddress Email { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public List<EmailAddress> Members { get; set; } = null!;
    public List<EmailAddress> Aliases { get; set; } = null!;
}