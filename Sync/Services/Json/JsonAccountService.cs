using System.Text.Json;
using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services.Json;

/// <summary>
///     Account service that reads and writes data to a JSON file.
///     Mainly used for testing purposes.
/// </summary>
public class JsonAccountService : ITarget
{
    private readonly Dictionary<EmailAddress, Group> _groups;

    private readonly string _path;

    private readonly Dictionary<EmailAddress, User> _users;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JsonAccountService" /> class.
    /// </summary>
    /// <param name="path">The path to the JSON file.</param>
    /// <exception cref="InvalidDataException">Thrown when the JSON data is invalid.</exception>
    public JsonAccountService(string path)
    {
        _path = path;
        if (!File.Exists(path))
        {
            _users = new Dictionary<EmailAddress, User>();
            _groups = new Dictionary<EmailAddress, Group>();
            return;
        }

        var json = File.ReadAllText(path);
        var jsonDataModel = JsonSerializer.Deserialize<JsonDataModel>(json) ??
                            throw new InvalidDataException("Invalid JSON data");
        _users = jsonDataModel.Users.ToDictionary(user => user.Email);
        _groups = jsonDataModel.Groups.ToDictionary(group => group.Email);
    }

    /// <summary>
    ///     Gets the users.
    /// </summary>
    /// <returns>A read-only dictionary of users.</returns>
    public Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync()
    {
        return Task.FromResult<IReadOnlyDictionary<EmailAddress, User>>(_users);
    }

    /// <summary>
    ///     Gets the users.
    /// </summary>
    /// <returns>A read-only dictionary of users.</returns>
    public Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync()
    {
        return Task.FromResult<IReadOnlyDictionary<EmailAddress, Group>>(_groups);
    }

    /// <summary>
    ///     Applies a user change.
    /// </summary>
    /// <param name="userChange">The user change.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the before user is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the after user conflicts with an existing user.</exception>
    public Task ApplyUserChangeAsync(UserChange userChange)
    {
        if (userChange.After is null && userChange.Before is null) return Task.CompletedTask;

        if (userChange.Before is not null)
            if (!_users.Remove(userChange.Before.Email))
                throw new KeyNotFoundException($"User '{userChange.Before.Email}' not found");

        if (userChange.After is not null) _users.Add(userChange.After.Email, userChange.After);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Applies a group change.
    /// </summary>
    /// <param name="groupChange">The group change.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the before group is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the after group conflicts with an existing group.</exception>
    public Task ApplyGroupChangeAsync(GroupChange groupChange)
    {
        if (groupChange.After is null && groupChange.Before is null) return Task.CompletedTask;

        if (groupChange.Before is not null)
            if (!_groups.Remove(groupChange.Before.Email))
                throw new KeyNotFoundException($"Group '{groupChange.Before.Email}' not found");

        if (groupChange.After is not null) _groups.Add(groupChange.After.Email, groupChange.After);

        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        var jsonDataModel = new JsonDataModel(_users.Values.ToList(), _groups.Values.ToList());
        var json = JsonSerializer.Serialize(jsonDataModel);
        await File.WriteAllTextAsync(_path, json);
    }

    private record JsonDataModel(List<User> Users, List<Group> Groups);
}