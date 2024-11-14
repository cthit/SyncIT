using SyncIT.Sync.Models;

namespace SyncIT.Sync;

public interface ITarget : IProvider
{
    public Task AddUserAsync(User user);
    public Task UpdateUserAsync(UserUpdate userUpdate);
    public Task RemoveUserAsync(User user);
    
    public Task AddGroupAsync(Group group);
    public Task UpdateGroupAsync(GroupUpdate groupUpdate);
    public Task RemoveGroupAsync(Group group);

} 