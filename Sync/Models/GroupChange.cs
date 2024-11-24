namespace SyncIT.Sync.Models;

public record GroupChange(Guid ChangeId, Group? Before, Group? After)
{
    public GroupChange(Group? before, Group? after) : this(Guid.NewGuid(), before, after)
    {
    }
}