using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models;

public class AdditionalUser
{
    [Key] public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<AdditionalUserAlias> Aliases { get; set; } = null!;
}