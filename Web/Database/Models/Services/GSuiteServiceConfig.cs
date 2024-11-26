using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models.Services;

public class GSuiteServiceConfig : BaseSyncServiceConfig
{
    public string AuthJson { get; set; } = null!;

    public string AdminEmail { get; set; } = null!;

    public bool IsReadOnly { get; set; } = true;


    public override bool CanBeTarget => !IsReadOnly;
}