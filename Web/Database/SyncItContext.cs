using Microsoft.EntityFrameworkCore;
using SyncIT.Web.Database.Models;
using SyncIT.Web.Database.Models.Services;

namespace SyncIT.Web.Database;

public class SyncItContext : DbContext
{
    public SyncItContext(DbContextOptions<SyncItContext> options) : base(options)
    {
    }


    public DbSet<AdditionalUser> AdditionalUsers { get; set; }
    public DbSet<AdditionalUserAlias> AdditionalUserAliases { get; set; }

    public DbSet<AdditionalGroup> AdditionalGroups { get; set; }
    public DbSet<AdditionalGroupAlias> AdditionalGroupAliases { get; set; }
    public DbSet<AdditionalGroupMember> AdditionalGroupMembers { get; set; }

    public DbSet<BaseSyncServiceConfig> BaseSyncServiceConfigs { get; set; }
    public DbSet<GSuiteServiceConfig> GSuiteServiceConfigs { get; set; }
    public DbSet<JsonServiceConfig> JsonServiceConfigs { get; set; }
    public DbSet<GammaServiceConfig> GammaServiceConfigs { get; set; }
}