using SyncIT.Sync.Models;

namespace SyncIT.Sync;

public interface ITarget : IProvider
{
    public Task ApplyUserChangeAsync(UserChange userChange);

    public Task ApplyGroupChangeAsync(GroupChange groupChange);
}