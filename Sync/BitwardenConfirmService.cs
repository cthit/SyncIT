using Microsoft.Extensions.Logging;

namespace SyncIT.Sync;

public class BitwardenConfirmService
{
    private readonly BwServeClient _bwServe;
    private readonly ILogger<BitwardenConfirmService> _logger;

    public BitwardenConfirmService(BwServeClient bwServe, ILogger<BitwardenConfirmService> logger)
    {
        _bwServe = bwServe;
        _logger = logger;
    }

    public async Task<List<PendingMember>> GetPendingMembersAsync(string organizationId, CancellationToken ct = default)
    {
        var members = await _bwServe.ListOrgMembersAsync(organizationId, ct);

        return members
            .Where(m => m.Status < 2)
            .Select(m => new PendingMember(
                m.Id,
                m.Email,
                m.Status switch
                {
                    0 => "Invited",
                    1 => "Accepted",
                    _ => $"Unknown ({m.Status})"
                }
            ))
            .ToList();
    }

    public async Task<ConfirmResult> ConfirmAllAsync(string organizationId, CancellationToken ct = default)
    {
        var pending = await GetPendingMembersAsync(organizationId, ct);
        var confirmed = 0;
        var errors = new List<string>();

        foreach (var member in pending)
        {
            if (ct.IsCancellationRequested)
                break;

            try
            {
                await _bwServe.ConfirmMemberAsync(organizationId, member.Id, ct);
                confirmed++;
                _logger.LogInformation("Confirmed {Email} in organization {OrgId}", member.Email, organizationId);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to confirm {member.Email}: {ex.Message}");
                _logger.LogError(ex, "Failed to confirm {Email} in organization {OrgId}", member.Email, organizationId);
            }
        }

        return new ConfirmResult(confirmed, errors);
    }

    public async Task<bool> HealthCheckAsync()
    {
        return await _bwServe.HealthCheckAsync();
    }
}
