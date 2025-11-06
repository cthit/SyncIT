using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services;

public static class ChangeApplier
{
    private const int DefaultMaxConcurrency = 5;

    public static async IAsyncEnumerable<UserChangeResult> ApplyUserChangesAsync(
        IReadOnlyList<UserChange> userChanges,
        ITarget target)
    {
        var factories = userChanges.Select(change =>
            (Func<Task<UserChangeResult>>)(() => ApplySafeUserChange(change, target)));
        await foreach (var result in RunWithMaxConcurrency(factories, DefaultMaxConcurrency).ConfigureAwait(false))
            yield return result;
    }

    public static async IAsyncEnumerable<GroupChangeResult> ApplyGroupChangesAsync(
        IReadOnlyList<GroupChange> groupChanges,
        ITarget target)
    {
        // We want to process additions, updates, and deletions in that order to avoid conflicts (e.g., trying to update a group by adding a group that does not exist yet).

        var additionChanges = groupChanges.Where(c => c.Before is null).ToList();
        var updateChanges = groupChanges.Where(c => c is { Before: not null, After: not null }).ToList();
        var deletionChanges = groupChanges.Where(c => c.After is null).ToList();

        var addFactories = additionChanges.Select(change =>
            (Func<Task<GroupChangeResult>>)(() => ApplySafeGroupChange(change, target)));
        var updateFactories = updateChanges.Select(change =>
            (Func<Task<GroupChangeResult>>)(() => ApplySafeGroupChange(change, target)));
        var delFactories = deletionChanges.Select(change =>
            (Func<Task<GroupChangeResult>>)(() => ApplySafeGroupChange(change, target)));


        await foreach (var result in RunWithMaxConcurrency(addFactories, DefaultMaxConcurrency)
                           .ConfigureAwait(false))
            yield return result;

        await foreach (var result in RunWithMaxConcurrency(updateFactories, DefaultMaxConcurrency)
                           .ConfigureAwait(false))
            yield return result;

        await foreach (var result in RunWithMaxConcurrency(delFactories, DefaultMaxConcurrency)
                           .ConfigureAwait(false))
            yield return result;
    }

    private static async IAsyncEnumerable<T> RunWithMaxConcurrency<T>(IEnumerable<Func<Task<T>>> taskFactories,
        int maxConcurrency)
    {
        using var enumerator = taskFactories.GetEnumerator();
        var running = new List<Task<T>>();

        while (running.Count < maxConcurrency && enumerator.MoveNext())
        {
            var task = enumerator.Current();
            running.Add(task);
        }

        while (running.Count > 0)
        {
            var completed = await Task.WhenAny(running).ConfigureAwait(false);
            running.Remove(completed);

            yield return await completed.ConfigureAwait(false);

            if (enumerator.MoveNext())
            {
                var next = enumerator.Current();
                running.Add(next);
            }
        }
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