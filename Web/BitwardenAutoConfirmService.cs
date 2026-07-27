using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SyncIT.Sync;
using SyncIT.Web.Database;
using SyncIT.Web.Database.Models;

namespace SyncIT.Web;

public class BitwardenAutoConfirmService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<BitwardenAutoConfirmService> _logger;
    private readonly TimeSpan _interval;

    public BitwardenAutoConfirmService(
        IServiceScopeFactory scopeFactory,
        ILoggerFactory loggerFactory,
        ILogger<BitwardenAutoConfirmService> logger,
        TimeSpan? interval = null)
    {
        _scopeFactory = scopeFactory;
        _loggerFactory = loggerFactory;
        _logger = logger;
        _interval = interval ?? TimeSpan.FromMinutes(15);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        _logger.LogInformation("Auto-confirm service started, interval: {Interval}", _interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunConfirmationCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-confirm cycle failed");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task RunConfirmationCycleAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SyncItContext>();

        var instances = await db.BitwardenInstances
            .Where(i => i.OrganizationId != null
                     && i.BotClientId != null
                     && i.BotClientSecret != null
                     && i.BotPassword != null)
            .ToListAsync(ct);

        if (instances.Count == 0)
            return;

        _logger.LogDebug("Auto-confirm cycle: checking {Count} instance(s)", instances.Count);

        foreach (var instance in instances)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                await ConfirmInstanceAsync(instance, db, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-confirm failed for instance {Instance}", instance.Name);
            }
        }
    }

    private async Task ConfirmInstanceAsync(BitwardenInstance instance, SyncItContext db, CancellationToken ct)
    {
        _logger.LogDebug("Confirming members for instance {Name} org {OrgId}",
            instance.Name, instance.OrganizationId);

        await using var cli = new BwCliService(
            _loggerFactory,
            instance.UrlBase.TrimEnd('/'),
            instance.BotClientId!,
            instance.BotClientSecret!,
            instance.BotPassword!);

        await cli.LoginAndUnlockAsync(ct);

        try
        {
            var members = await cli.ListOrgMembersAsync(instance.OrganizationId!, ct);
            var pending = members.Where(m => m.Status < 2).ToList();

            if (pending.Count == 0)
            {
                _logger.LogInformation("No pending members for {Name}", instance.Name);
                return;
            }

            _logger.LogDebug("Found {Count} pending member(s) for {Name}", pending.Count, instance.Name);

            var confirmed = 0;
            foreach (var member in pending)
            {
                if (ct.IsCancellationRequested) break;

                try
                {
                    await cli.ConfirmMemberAsync(instance.OrganizationId!, member.Id, ct);
                    confirmed++;
                    _logger.LogDebug("Auto-confirmed {Email} in {Org}", member.Email, instance.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to auto-confirm {Email} in {Org}",
                        member.Email, instance.Name);
                }
            }

            instance.LastConfirmDate = DateTime.UtcNow;
            instance.LastConfirmCount = confirmed;
            await db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Auto-confirm for {Name}: {Confirmed} confirmed of {Total} pending",
                instance.Name, confirmed, pending.Count);
        }
        finally
        {
            await cli.LockAsync();
        }
    }
}
