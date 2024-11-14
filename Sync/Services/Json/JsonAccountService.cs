using System.Text.Json;
using SyncIT.Sync.Models;

namespace SyncIT.Sync.Services.Json;

/// <summary>
/// Account service that reads and writes data to a JSON file.
/// Mainly used for testing purposes.
/// </summary>
public class JsonAccountService : ITarget
{
 
    private record JsonDataModel(List<User> Users, List<Group> Groups);
    
    private readonly Dictionary<EmailAddress, User> _users;
    private readonly Dictionary<EmailAddress, Group> _groups;
    
    private readonly string _path;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAccountService"/> class.
    /// </summary>
    /// <param name="path">The path to the JSON file.</param>
    /// <exception cref="InvalidDataException">Thrown when the JSON data is invalid.</exception>
    public JsonAccountService(string path)
    {
        _path = path;
        if (!File.Exists(path))
        {
            _users = new();
            _groups = new();
            return;
        }
        
        var json = File.ReadAllText(path);
        JsonDataModel jsonDataModel = JsonSerializer.Deserialize<JsonDataModel>(json) ?? throw new InvalidDataException("Invalid JSON data");
        _users = jsonDataModel.Users.ToDictionary(user => user.Email);
        _groups = jsonDataModel.Groups.ToDictionary(group => group.Email);
    }

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <returns>A read-only dictionary of users.</returns>
    public Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync()
    {
        return Task.FromResult<IReadOnlyDictionary<EmailAddress, User>>(_users);
    }

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <returns>A read-only dictionary of users.</returns>
    public Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync()
    {
        return Task.FromResult<IReadOnlyDictionary<EmailAddress, Group>>(_groups);
    }


    /// <summary>
    /// Adds a user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    public Task AddUserAsync(User user)
    {
        _users.Add(user.Email, user);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates a user. Keyed by before user email.
    /// </summary>
    /// <param name="userUpdate">The user update information.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
    public Task UpdateUserAsync(UserUpdate userUpdate)
    {
        // For this target we could ignore this but in a real world scenario we would need to check if the user exists before updating it.
        if (!_users.Remove(userUpdate.After.Email))
        {
            throw new KeyNotFoundException("User not found " + userUpdate.Before.Email);
        }
        
        _users.Add(userUpdate.After.Email, userUpdate.After);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a user. Keyed by email.
    /// </summary>
    /// <param name="user">The user to remove.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
    public Task RemoveUserAsync(User user)
    {
        if (!_users.Remove(user.Email))
        {
            throw new KeyNotFoundException("User not found " + user.Email);
        }
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Adds a group.
    /// </summary>
    /// <param name="group">The group to add.</param>
    public Task AddGroupAsync(Group group)
    {
        _groups.Add(group.Email, group);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates a group. Keyed by before group email.
    /// </summary>
    /// <param name="groupUpdate">The group update information.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the group is not found.</exception>
    public Task UpdateGroupAsync(GroupUpdate groupUpdate)
    {
        // For this target we could ignore this but in a real world scenario we would need to check if the group exists before updating it.
        if (!_groups.Remove(groupUpdate.Before.Email))
        {
            throw new KeyNotFoundException("Group not found " + groupUpdate.Before.Email);
        }
        
        _groups.Add(groupUpdate.After.Email, groupUpdate.After);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a group. Keyed by group email.
    /// </summary>
    /// <param name="group">The group to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the group is not found.</exception>
    public Task RemoveGroupAsync(Group group)
    {
        if (!_groups.Remove(group.Email))
        {
            throw new KeyNotFoundException("Group not found " + group.Email);
        }
        return Task.CompletedTask;
    }
    
    public async Task SaveAsync()
    {
        var jsonDataModel = new JsonDataModel(_users.Values.ToList(), _groups.Values.ToList());
        var json = JsonSerializer.Serialize(jsonDataModel);
        await File.WriteAllTextAsync(_path, json);
    }
    
}