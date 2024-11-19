using SyncIT.Sync.Models;
using SyncIT.Sync.Utils;

namespace SyncIT.Sync.Services.Gamma;

/// <summary>
///     Account source from Gamma v2 also known as "Auth"
/// </summary>
public class GammaAccountService : IProvider
{
    private readonly GammaAccountScaffoldApi _gammaAccountScaffoldApi;
    private readonly GammaAccountServiceSettings _settings;

    public GammaAccountService(GammaAccountServiceSettings settings, HttpClient httpClient)
    {
        _gammaAccountScaffoldApi = new GammaAccountScaffoldApi(httpClient, settings.BaseUrl, settings.ApiKey);
        _settings = settings;
    }

    public async Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync()
    {
        var gammaUsers = await _gammaAccountScaffoldApi.GetUsersAsync();

        return gammaUsers.Select(gammaUser => new User(
            gammaUser.Cid,
            gammaUser.FirstName,
            gammaUser.LastName,
            gammaUser.Nick,
            CreateSanitizedEmail(gammaUser.Cid),
            new HashSet<EmailAddress>
                { CreateSanitizedEmail(gammaUser.Nick) }
        )).ToDictionary(user => user.Email);
    }

    public async Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync()
    {
        var gammaSuperGroups = await _gammaAccountScaffoldApi.GetSuperGroupsAsync();

        Dictionary<EmailAddress, Group> groups = new();

        foreach (var gammaSuperGroup in gammaSuperGroups)
        {
            var superGroup = new Group(
                CreateSanitizedEmail(gammaSuperGroup.Name),
                new HashSet<EmailAddress>(),
                gammaSuperGroup.Type,
                new HashSet<EmailAddress>());

            foreach (var gammaGroup in gammaSuperGroup.Groups)
            {
                var memberGroup = new Group(
                    CreateSanitizedEmail(gammaGroup.Name),
                    new HashSet<EmailAddress>(),
                    string.Empty,
                    new HashSet<EmailAddress>());

                foreach (var gammaPostUser in gammaGroup.Members)
                {
                    EmailAddress memberEmail = gammaSuperGroup.UseManagedAccount
                        ? CreateSanitizedEmail(gammaPostUser.User.Cid)
                        : gammaPostUser.User.Email;
                    memberGroup.Members.Add(memberEmail);
                    if (gammaPostUser.Post.EmailPrefix == "")
                        continue;

                    var groupPostEmail = CreateSanitizedEmail($"{gammaPostUser.Post.EmailPrefix}.{gammaGroup.Name}");
                    var superGroupPostEmail =
                        CreateSanitizedEmail($"{gammaPostUser.Post.EmailPrefix}.{gammaSuperGroup.Name}");

                    if (groups.TryGetValue(groupPostEmail, out var groupPostGroup))
                        groupPostGroup.Members.Add(memberEmail);
                    else
                        groups.Add(groupPostEmail,
                            new Group(groupPostEmail, new HashSet<EmailAddress>(), string.Empty,
                                new HashSet<EmailAddress> { memberEmail }));

                    if (groups.TryGetValue(superGroupPostEmail, out var superGroupPostGroup))
                        superGroupPostGroup.Members.Add(groupPostEmail);
                    else
                        groups.Add(superGroupPostEmail,
                            new Group(superGroupPostEmail, new HashSet<EmailAddress>(), string.Empty,
                                new HashSet<EmailAddress> { groupPostEmail }));
                }

                groups.Add(memberGroup.Email, memberGroup);
            }

            groups.Add(superGroup.Email, superGroup);
        }

        return groups;
    }

    private EmailAddress CreateSanitizedEmail(string localPart)
    {
        return new EmailAddress($"{EmailSanitizer.SanitizeLocal(localPart)}@{_settings.AccountEmailDomain}");
    }
}