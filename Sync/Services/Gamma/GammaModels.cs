namespace SyncIT.Sync.Services.Gamma;

public record GammaUser(string Email, string Cid, string FirstName, string LastName, string Nick);

public record GammaPost(Guid PostId, string SvText, string EnText, string EmailPrefix);

public record GammaPostUser(GammaPost Post, GammaUser User);

public record GammaGroup(string Name, string PrettyName, GammaPostUser[] Members);

public record GammaSuperGroup(string Name, string PrettyName, string Type, GammaGroup[] Groups, bool UseManagedAccount);
