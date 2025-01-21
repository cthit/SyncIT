using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SyncIT.Sync.Models;
using SyncIT.Web.Database.Models;
using SyncIT.Web.Database.Models.Services;

namespace SyncIT.Web.Database;

public class SyncItContext : DbContext
{
    public SyncItContext(DbContextOptions<SyncItContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var emailConverter = new ValueConverter<EmailAddress, string>(
            v => v.Email,
            v => new EmailAddress(v)
        );
        
        modelBuilder.Entity<AdditionalUser>()
            .Property(e => e.Email)
            .HasConversion(emailConverter);
        
        modelBuilder.Entity<AdditionalUserAlias>()
            .Property(e => e.UserEmail)
            .HasConversion(emailConverter);
        modelBuilder.Entity<AdditionalUserAlias>()
            .Property(e => e.AliasEmail)
            .HasConversion(emailConverter);
        
        modelBuilder.Entity<AdditionalGroup>()
            .Property(e => e.Email)
            .HasConversion(emailConverter);
        
        modelBuilder.Entity<AdditionalGroupAlias>()
            .Property(e => e.GroupEmail)
            .HasConversion(emailConverter);
        modelBuilder.Entity<AdditionalGroupAlias>()
            .Property(e => e.AliasEmail)
            .HasConversion(emailConverter);
        
        modelBuilder.Entity<AdditionalGroupMember>()
            .Property(e => e.GroupEmail)
            .HasConversion(emailConverter);
        modelBuilder.Entity<AdditionalGroupMember>()
            .Property(e => e.MemberEmail)
            .HasConversion(emailConverter);
            
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