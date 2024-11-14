namespace SyncIT.Sync.Models;

/// <summary>
/// Represents a group of users.
/// </summary>
/// <param name="Email">The email address of the group.</param>
/// <param name="Type">The type of the group.</param>
/// <param name="Members">List of member emails</param>
public record Group(EmailAddress Email, ISet<EmailAddress> Aliases, string Type, ISet<EmailAddress> Members);