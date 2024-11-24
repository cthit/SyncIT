using Microsoft.EntityFrameworkCore;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(GroupEmail), nameof(AliasEmail))]
public class AdditionalGroupAlias
{
    public string GroupEmail { get; set; } = null!;
    public string AliasEmail { get; set; } = null!;
}