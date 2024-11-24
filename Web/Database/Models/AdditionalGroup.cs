using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models;

public class AdditionalGroup
{
    [Key]
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    public ICollection<AdditionalGroupMember> Members { get; set; } = null!;
    public ICollection<AdditionalGroupAlias> Aliases { get; set; } = null!;
}