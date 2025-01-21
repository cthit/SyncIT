using System.ComponentModel.DataAnnotations;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

public class AdditionalGroup
{
    [Key] public EmailAddress Email { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<AdditionalGroupMember> Members { get; set; } = null!;
    public ICollection<AdditionalGroupAlias> Aliases { get; set; } = null!;
    
}