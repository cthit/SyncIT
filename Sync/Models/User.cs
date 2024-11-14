namespace SyncIT.Sync.Models;

public record User(string Cid, string FirstName, string LastName, string Nick, EmailAddress Email, ISet<EmailAddress> Aliases);