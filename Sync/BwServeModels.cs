namespace SyncIT.Sync;

public record BwMember(
    string Id,
    string Email,
    int Status,
    int Type
);

public record PendingMember(string Id, string Email, string Status);

public record ConfirmResult(int Confirmed, List<string> Errors);
