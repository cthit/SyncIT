using System.Net;
using Google;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Requests;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SyncIT.Sync.Models;
using Group = SyncIT.Sync.Models.Group;
using User = SyncIT.Sync.Models.User;

namespace SyncIT.Sync.Services.GSuite;

public class GSuiteAccountService : ITarget
{
    private static readonly string[] Scopes =
        [DirectoryService.Scope.AdminDirectoryUser, DirectoryService.Scope.AdminDirectoryGroup];

    private readonly EmailAddress _adminEmail;
    private readonly DirectoryService _directoryService;
    private readonly string _googleCustomer = "my_customer";

    private readonly ILogger<GSuiteAccountService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;


    public GSuiteAccountService(string authJson, EmailAddress adminEmail, ILogger<GSuiteAccountService> logger)
    {
        _logger = logger;
        var credential = GoogleCredential.FromJson(authJson).CreateScoped(Scopes).CreateWithUser(adminEmail);
        _directoryService = new DirectoryService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "SyncIT"
        });
        _adminEmail = adminEmail;

        _retryPolicy = Policy.Handle<GoogleApiException>(IsTransient)
            .WaitAndRetryAsync(5,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(1000)),
                (exception, timespan, retryAttempt, _) => _logger.LogDebug(exception,
                    "Retry nr {retryAttempt} encountered an error. Waiting {timespan} before next retry.", retryAttempt,
                    timespan));
    }

    public async Task<IReadOnlyDictionary<EmailAddress, User>> GetUsersAsync()
    {
        var request = _directoryService.Users.List();
        request.Customer = _googleCustomer;

        var response = await _retryPolicy.ExecuteAsync(request.ExecuteAsync).ConfigureAwait(false);

        Dictionary<EmailAddress, User> users = new();

        while (true)
        {
            foreach (var user in response.UsersValue)
            {
                var primaryEmail = new EmailAddress(user.PrimaryEmail);
                if (primaryEmail.Equals(_adminEmail)) continue;

                var recoveryEmail = string.IsNullOrWhiteSpace(user.RecoveryEmail)
                    ? null
                    : new EmailAddress(user.RecoveryEmail);
                var nick = user.Name.GivenName.Contains('/') ? user.Name.GivenName.Split('/')[0].Trim() : string.Empty;
                users.Add(primaryEmail, new User(
                    user.PrimaryEmail.Split('@')[0],
                    user.Name.GivenName,
                    user.Name.FamilyName,
                    nick,
                    primaryEmail,
                    recoveryEmail,
                    user.Aliases?.Select(alias => new EmailAddress(alias))?.ToHashSet() ?? []
                ));
            }

            if (response.NextPageToken is null) break;

            request.PageToken = response.NextPageToken;
            response = await _retryPolicy.ExecuteAsync(request.ExecuteAsync).ConfigureAwait(false);
        }

        return users;
    }

    public async Task<IReadOnlyDictionary<EmailAddress, Group>> GetGroupsAsync()
    {
        var request = _directoryService.Groups.List();
        request.Customer = _googleCustomer;

        var response = await _retryPolicy.ExecuteAsync(request.ExecuteAsync).ConfigureAwait(false);

        Dictionary<EmailAddress, Group> groups = new();

        while (true)
        {
            var groupTasks = response.GroupsValue.Select(GetGroupWithMembers);
            await foreach(var groupTask in Task.WhenEach(groupTasks))
            {
                var group = await groupTask; 
                groups.Add(group.Email, group);
            }

            if (response.NextPageToken is null) break;

            request.PageToken = response.NextPageToken;
            response = await _retryPolicy.ExecuteAsync(request.ExecuteAsync).ConfigureAwait(false);
        }

        return groups;
    }

    public async Task ApplyUserChangeAsync(UserChange userChange)
    {
        switch (userChange)
        {
            case { Before: not null, After: null }:
                //Delete user
                await DeleteUserAsync(userChange.Before);
                break;
            case { After: not null, Before: null }:
                await CreateUserAsync(userChange.After);
                break;
            case { Before: not null, After: not null }:
                await UpdateUserAsync(userChange);
                break;
        }
    }

    public async Task ApplyGroupChangeAsync(GroupChange groupChange)
    {
        switch (groupChange)
        {
            case { Before: not null, After: null }:
                //Delete group
                await DeleteGroupAsync(groupChange.Before);
                break;
            case { After: not null, Before: null }:
                await CreateGroupAsync(groupChange.After);
                break;
            case { Before: not null, After: not null }:
                await UpdateGroupAsync(groupChange);
                break;
        }
    }

    private async Task UpdateUserAsync(UserChange userChange)
    {
        if (userChange.After is null || userChange.Before is null)
            throw new ArgumentException("Both before and after user must be provided");

        var updateUser = ToGoogleUser(userChange.After);
        var request = _directoryService.Users.Update(updateUser, userChange.Before.Email);
        await _retryPolicy.ExecuteAsync(request.ExecuteAsync).ConfigureAwait(false);

        foreach (var beforeAlias in userChange.Before.Aliases)
            if (!userChange.After.Aliases.Contains(beforeAlias))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Users.Aliases.Delete(userChange.Before.Email, beforeAlias)
                        .ExecuteAsync).ConfigureAwait(false);

        foreach (var afterAlias in userChange.After.Aliases)
            if (!userChange.Before.Aliases.Contains(afterAlias))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Users.Aliases
                        .Insert(new Alias { AliasValue = afterAlias }, userChange.Before.Email).ExecuteAsync)
                    .ConfigureAwait(false);
    }

    private async Task CreateUserAsync(User user)
    {
        var newUser = ToGoogleUser(user);
        await _retryPolicy.ExecuteAsync(_directoryService.Users.Insert(newUser).ExecuteAsync).ConfigureAwait(false);

        //TODO: Do we need some delay/logic for managing propagation delay?
        foreach (var alias in user.Aliases)
            await _retryPolicy
                .ExecuteAsync(_directoryService.Users.Aliases.Insert(new Alias { AliasValue = alias }, user.Email)
                    .ExecuteAsync).ConfigureAwait(false);
    }

    private async Task DeleteUserAsync(User user)
    {
        await _retryPolicy.ExecuteAsync(_directoryService.Users.Delete(user.Email).ExecuteAsync).ConfigureAwait(false);
    }

    private async Task UpdateGroupAsync(GroupChange groupChange)
    {
        if (groupChange.After is null || groupChange.Before is null)
            throw new ArgumentException("Both before and after group must be provided");

        var updateGroup = new Google.Apis.Admin.Directory.directory_v1.Data.Group
        {
            Email = groupChange.After.Email
        };

        await _retryPolicy
            .ExecuteAsync(_directoryService.Groups.Update(updateGroup, groupChange.Before.Email).ExecuteAsync)
            .ConfigureAwait(false);

        foreach (var beforeAlias in groupChange.Before.Aliases)
            if (!groupChange.After.Aliases.Contains(beforeAlias))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Groups.Aliases.Delete(groupChange.Before.Email, beforeAlias)
                        .ExecuteAsync).ConfigureAwait(false);

        foreach (var afterAlias in groupChange.After.Aliases)
            if (!groupChange.Before.Aliases.Contains(afterAlias))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Groups.Aliases
                        .Insert(new Alias { AliasValue = afterAlias }, groupChange.Before.Email).ExecuteAsync)
                    .ConfigureAwait(false);

        var beforeMembers = groupChange.Before.Members;
        var afterMembers = groupChange.After.Members;

        foreach (var beforeMember in beforeMembers)
            if (!afterMembers.Contains(beforeMember))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Members.Delete(groupChange.Before.Email, beforeMember).ExecuteAsync)
                    .ConfigureAwait(false);

        foreach (var afterMember in afterMembers)
            if (!beforeMembers.Contains(afterMember))
                await _retryPolicy
                    .ExecuteAsync(_directoryService.Members
                        .Insert(new Member { Email = afterMember }, groupChange.Before.Email).ExecuteAsync)
                    .ConfigureAwait(false);
    }

    private async Task CreateGroupAsync(Group group)
    {
        var newGroup = new Google.Apis.Admin.Directory.directory_v1.Data.Group
        {
            Email = group.Email,
            Name = group.Name
        };

        await _retryPolicy.ExecuteAsync(_directoryService.Groups.Insert(newGroup).ExecuteAsync).ConfigureAwait(false);

        //TODO: Do we need some delay/logic for managing propagation delay?

        foreach (var alias in group.Aliases)
            await _retryPolicy
                .ExecuteAsync(_directoryService.Groups.Aliases.Insert(new Alias { AliasValue = alias }, group.Email)
                    .ExecuteAsync).ConfigureAwait(false);

        foreach (var member in group.Members)
            await _retryPolicy
                .ExecuteAsync(_directoryService.Members.Insert(new Member { Email = member }, group.Email).ExecuteAsync)
                .ConfigureAwait(false);
    }

    private async Task DeleteGroupAsync(Group group)
    {
        await _retryPolicy.ExecuteAsync(_directoryService.Groups.Delete(group.Email).ExecuteAsync)
            .ConfigureAwait(false);
    }

    private async Task<List<Member>> GetGroupMembers(string groupEmail)
    {
        var membersRequest = _directoryService.Members.List(groupEmail);
        var membersResponse = await _retryPolicy.ExecuteAsync(membersRequest.ExecuteAsync).ConfigureAwait(false);

        List<Member> members = new();

        while (true)
        {
            if (membersResponse is null) throw new Exception(); //TODO

            members.AddRange(membersResponse.MembersValue ?? Array.Empty<Member>());

            if (membersResponse.NextPageToken is null) break;

            membersRequest.PageToken = membersResponse.NextPageToken;
            membersResponse = await _retryPolicy.ExecuteAsync(membersRequest.ExecuteAsync).ConfigureAwait(false);
        }

        return members;
    }
    
    private async Task<Group> GetGroupWithMembers(Google.Apis.Admin.Directory.directory_v1.Data.Group googleGroup)
    {
        _logger.LogInformation("Getting group {groupEmail}", googleGroup.Email);
        var members = await GetGroupMembers(googleGroup.Email);
        _logger.LogInformation("Got {memberCount} members for group {groupEmail}", members.Count, googleGroup.Email);
        var primaryEmail = new EmailAddress(googleGroup.Email);
        return new Group(
            primaryEmail,
            googleGroup.Aliases?.Select(alias => new EmailAddress(alias))?.ToHashSet() ?? [],
            googleGroup.Name,
            members.Select(member => new EmailAddress(member.Email))?.ToHashSet() ?? []
        );
    }

    private static Google.Apis.Admin.Directory.directory_v1.Data.User ToGoogleUser(User user)
    {
        return new Google.Apis.Admin.Directory.directory_v1.Data.User
        {
            Name = new UserName
            {
                GivenName = $"{user.Nick} / {user.FirstName}",
                FamilyName = user.LastName
            },
            PrimaryEmail = user.Email,
            RecoveryEmail = user.RecoveryEmail?.Email ?? string.Empty
        };
    }

    private static bool IsTransient(GoogleApiException exception)
    {
        return exception.HttpStatusCode switch
        {
            HttpStatusCode.Forbidden => exception.Error?.Errors?.SingleOrDefault()?.Reason == "quotaExceeded",
            HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable => true,
            _ => false
        };
    }
}