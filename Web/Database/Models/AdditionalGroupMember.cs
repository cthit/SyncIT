using Microsoft.EntityFrameworkCore;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(GroupEmail), nameof(MemberEmail))]
public class AdditionalGroupMember
{
    public string GroupEmail { get; set; } = null!;
    public string MemberEmail { get; set; } = null!;
    
}