namespace SyncIT.Sync.Models;

public record User(
    string Cid,
    string FirstName,
    string LastName,
    string Nick,
    EmailAddress Email,
    EmailAddress? RecoveryEmail,
    ISet<EmailAddress> Aliases)
{
    public virtual bool Equals(User? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Cid == other.Cid && FirstName == other.FirstName && LastName == other.LastName && Nick == other.Nick &&
               Email.Equals(other.Email) && Equals(RecoveryEmail, other.RecoveryEmail) && Aliases.SetEquals(other.Aliases);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Cid, FirstName, LastName, Nick, Email, RecoveryEmail, Aliases);
    }
}