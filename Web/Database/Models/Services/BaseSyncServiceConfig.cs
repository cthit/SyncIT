using System.ComponentModel.DataAnnotations.Schema;
using SyncIT.Sync.Services;

namespace SyncIT.Web.Database.Models.Services;

public abstract class BaseSyncServiceConfig
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    [NotMapped] public abstract bool CanBeTarget { get; }

    public abstract ISource ToSource(IServiceProvider serviceProvider);
}