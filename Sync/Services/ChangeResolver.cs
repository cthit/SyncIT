using System.Diagnostics.Contracts;
using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services;

public class ChangeResolver
{
    [Pure]
    public List<GroupChange> ResolveGroupChanges(IReadOnlyDictionary<EmailAddress, Group> before,
        IReadOnlyDictionary<EmailAddress, Group> after)
    {
        var changes = new List<GroupChange>();


        foreach (var beforeGroup in before)
            if (!after.TryGetValue(beforeGroup.Key, out var afterGroup))
                // Group was removed
                changes.Add(new GroupChange(beforeGroup.Value, null));
            else if (!beforeGroup.Value.Equals(afterGroup))
                // Group was modified
                changes.Add(new GroupChange(beforeGroup.Value, afterGroup));

        foreach (var afterGroup in after)
            if (!before.ContainsKey(afterGroup.Key))
                // Group was added
                changes.Add(new GroupChange(null, afterGroup.Value));

        return changes;
    }

    [Pure]
    public List<UserChange> ResolveUserChanges(IReadOnlyDictionary<EmailAddress, User> before,
        IReadOnlyDictionary<EmailAddress, User> after)
    {
        var changes = new List<UserChange>();

        foreach (var beforeUser in before)
            if (!after.TryGetValue(beforeUser.Key, out var afterUser))
                // User was removed
                changes.Add(new UserChange(beforeUser.Value, null));
            else if (!beforeUser.Value.Equals(afterUser))
                // User was modified
                changes.Add(new UserChange(beforeUser.Value, afterUser));

        foreach (var afterUser in after)
            if (!before.ContainsKey(afterUser.Key))
                // User was added
                changes.Add(new UserChange(null, afterUser.Value));

        return changes;
    }
}