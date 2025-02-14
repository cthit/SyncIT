namespace SyncIT.Sync.Models;

public record AdditionalUser(
    User User,
    bool OverwriteFirstName,
    bool OverwriteNick,
    bool OverwriteLastName,
    bool OverwriteRecoveryEmail);