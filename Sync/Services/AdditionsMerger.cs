using SyncIT.Sync.Models;

namespace SyncIT.Sync;

public class AdditionsMerger
{
    public static IReadOnlyDictionary<EmailAddress, User> MergeAdditionalUsers(
        IReadOnlyDictionary<EmailAddress, User> sourceUsers, IEnumerable<AdditionalUser> additionalUsers)
    {
        var result = new Dictionary<EmailAddress, User>(sourceUsers);
        foreach (var additionalUser in additionalUsers)
        {
            var email = additionalUser.User.Email;
            if (result.TryGetValue(email, out var user))
            {
                user.Aliases.UnionWith(additionalUser.User.Aliases);
                result[email] = user with
                {
                    FirstName = additionalUser.OverwriteFirstName ? additionalUser.User.FirstName : user.FirstName,
                    Nick = additionalUser.OverwriteNick ? additionalUser.User.Nick : user.Nick,
                    LastName = additionalUser.OverwriteLastName ? additionalUser.User.LastName : user.LastName,
                    RecoveryEmail = additionalUser.OverwriteRecoveryEmail
                        ? additionalUser.User.RecoveryEmail
                        : user.RecoveryEmail
                };
            }
            else
            {
                result[email] = additionalUser.User;
            }
        }

        return result;
    }

    public static IReadOnlyDictionary<EmailAddress, Group> MergeAdditionalGroups(
        IReadOnlyDictionary<EmailAddress, Group> sourceGroups, IEnumerable<AdditionalGroup> additionalGroups)
    {
        var result = new Dictionary<EmailAddress, Group>(sourceGroups);
        foreach (var additionalGroup in additionalGroups)
        {
            var email = additionalGroup.Group.Email;
            if (result.TryGetValue(email, out var group))
            {
                group.Aliases.UnionWith(additionalGroup.Group.Aliases);
                group.Members.UnionWith(additionalGroup.Group.Members);
                result[email] = group with
                {
                    Name = additionalGroup.OverwriteName ? additionalGroup.Group.Name : group.Name
                };
            }
            else
            {
                result[email] = additionalGroup.Group;
            }
        }

        return result;
    }
}