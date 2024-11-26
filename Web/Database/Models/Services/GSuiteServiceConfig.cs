using System.ComponentModel.DataAnnotations;

namespace SyncIT.Web.Database.Models.Services;

public class GSuiteServiceConfig : BaseSyncServiceConfig
{
    [MinLength(10)] public string AuthJson { get; set; } = null!;

    [EmailAddress] public string AdminEmail { get; set; } = null!;

    public bool IsReadOnly { get; set; } = true;


    public override bool CanBeTarget => !IsReadOnly;
}