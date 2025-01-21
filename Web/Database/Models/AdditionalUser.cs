using System.ComponentModel.DataAnnotations;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalUser
{
    [Key] public EmailAddress Email { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<AdditionalUserAlias> Aliases { get; set; } = null!;
}