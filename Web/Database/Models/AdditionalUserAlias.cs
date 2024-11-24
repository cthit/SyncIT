using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SyncIT.Web.Database.Models;

[PrimaryKey(nameof(UserEmail), nameof(AliasEmail))]
public class AdditionalUserAlias
{
    public string UserEmail { get; set; } = null!;
    public string AliasEmail { get; set; } = null!;
}