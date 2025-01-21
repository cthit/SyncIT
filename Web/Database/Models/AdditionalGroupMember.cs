using Microsoft.EntityFrameworkCore;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(GroupEmail), nameof(MemberEmail))]
public class AdditionalGroupMember
{
    public EmailAddress GroupEmail { get; set; } = null!;
    public EmailAddress MemberEmail { get; set; } = null!;
}