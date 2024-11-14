using SyncIT.Sync.Models;

namespace SyncIT.Sync;

public interface IProvider
{
    public Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync();
    public Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync();
}