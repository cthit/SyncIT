namespace SyncIT.Sync.Models;

public record UserChange(Guid ChangeId, User? Before, User? After)
{
    public UserChange(User? before, User? after) : this(Guid.NewGuid(), before, after)
    {
    }
}