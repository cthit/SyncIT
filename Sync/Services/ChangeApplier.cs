using System.Runtime.CompilerServices;
using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services;

public class ChangeApplier
{
    public async IAsyncEnumerable<UserChangeResult> ApplyUserChangesAsync(
        IReadOnlyList<UserChange> userChanges,
        ITarget target,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var userChange in userChanges)
        {
            Exception? exception = null;
            try
            {
                await target.ApplyUserChangeAsync(userChange).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            yield return new UserChangeResult(userChange, exception);
        }
    }

    public async IAsyncEnumerable<GroupChangeResult> ApplyGroupChangesAsync(
        IReadOnlyList<GroupChange> groupChanges,
        ITarget target,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var groupChange in groupChanges)
        {
            Exception? exception = null;
            try
            {
                await target.ApplyGroupChangeAsync(groupChange).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            yield return new GroupChangeResult(groupChange, exception);
        }
    }
}