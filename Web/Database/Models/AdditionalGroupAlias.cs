using Microsoft.EntityFrameworkCore;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(GroupEmail), nameof(AliasEmail))]
public class AdditionalGroupAlias
{
    public EmailAddress GroupEmail { get; set; } = null!;
    public EmailAddress AliasEmail { get; set; } = null!;
}