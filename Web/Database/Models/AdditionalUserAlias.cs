using Microsoft.EntityFrameworkCore;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(UserEmail), nameof(AliasEmail))]
public class AdditionalUserAlias
{
    public EmailAddress UserEmail { get; set; } = null!;
    public EmailAddress AliasEmail { get; set; } = null!;
}