using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services;

public interface ITarget : ISource
{
    public Task ApplyUserChangeAsync(UserChange userChange);

    public Task ApplyGroupChangeAsync(GroupChange groupChange);
}