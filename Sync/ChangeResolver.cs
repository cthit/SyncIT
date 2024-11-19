using SyncIT.Sync.Models;

namespace SyncIT.Sync;

public class ChangeResolver
{
    public List<GroupChange> ResolveGroupChanges(IReadOnlyDictionary<EmailAddress, Group> before,
        IReadOnlyDictionary<EmailAddress, Group> after)
    {
        var changes = new List<GroupChange>();


        foreach (var beforeGroup in before)
            if (!after.TryGetValue(beforeGroup.Key, out var afterGroup))
                changes.Add(new GroupChange(beforeGroup.Value, null));
            else if (!beforeGroup.Value.Equals(afterGroup)) changes.Add(new GroupChange(beforeGroup.Value, afterGroup));

        foreach (var afterGroup in after)
            if (!before.ContainsKey(afterGroup.Key))
                changes.Add(new GroupChange(null, afterGroup.Value));

        return changes;
    }