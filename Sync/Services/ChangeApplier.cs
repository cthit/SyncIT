using System.Runtime.CompilerServices;
using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services;

public static class ChangeApplier
{
    public static IAsyncEnumerable<Task<UserChangeResult>> ApplyUserChangesAsync(
        IReadOnlyList<UserChange> userChanges,
        ITarget target)
    {
        var changeTasks = userChanges.Select(change => ApplySafeUserChange(change, target)).ToList();
        
        return Task.WhenEach(changeTasks);
    }

    public static IAsyncEnumerable<Task<GroupChangeResult>> ApplyGroupChangesAsync(
        IReadOnlyList<GroupChange> groupChanges,
        ITarget target)
    {
        var changeTasks = groupChanges.Select(change => ApplySafeGroupChange(change, target)).ToList();
        
        return Task.WhenEach(changeTasks);
    }
    
    private static async Task<UserChangeResult> ApplySafeUserChange(UserChange userChange, ITarget target)
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

        return new UserChangeResult(userChange, exception);
    }
    
    private static async Task<GroupChangeResult> ApplySafeGroupChange(GroupChange groupChange, ITarget target)
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

        return new GroupChangeResult(groupChange, exception);
    }
}