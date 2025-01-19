namespace SyncIT.Sync.Models;

/// <summary>
///     Represents a group of users.
/// </summary>
/// <param name="Email">The email address of the group.</param>
/// <param name="Aliases">List of alias emails</param>
/// <param name="Name">The name of the group.</param>
/// <param name="Members">List of member emails</param>
public record Group(EmailAddress Email, ISet<EmailAddress> Aliases, string Name, ISet<EmailAddress> Members)
{
    public virtual bool Equals(Group? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Email.Equals(other.Email) && Name == other.Name && Aliases.SetEquals(other.Aliases) &&
               Members.SetEquals(other.Members);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Email, Aliases, Name, Members);
    }
}