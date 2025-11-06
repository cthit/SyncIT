using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    public DbSet<AdditionalDbUser> AdditionalUsers { get; set; }
    public DbSet<AdditionalDbGroup> AdditionalGroups { get; set; }

    public DbSet<BaseSyncServiceConfig> BaseSyncServiceConfigs { get; set; }
    public DbSet<GSuiteServiceConfig> GSuiteServiceConfigs { get; set; }
    public DbSet<JsonServiceConfig> JsonServiceConfigs { get; set; }
    public DbSet<GammaServiceConfig> GammaServiceConfigs { get; set; }

    public DbSet<BitwardenInstance> BitwardenInstances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var emailConverter = new ValueConverter<EmailAddress, string>(
            v => v.Email,
            v => new EmailAddress(v)
        );

        var nullableEmailConverter = new ValueConverter<EmailAddress?, string?>(
            v => v == null ? null : v.Email,
            v => v == null ? null : new EmailAddress(v)
        );

        var emailListConverter = new ValueConverter<List<EmailAddress>, string>(
            v => string.Join(";", v.Select(e => e.Email)),
            v => v.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(e => new EmailAddress(e)).ToList()
        );

        var emailListValueComparer = new ValueComparer<List<EmailAddress>>(
            (c1, c2) => (c1 ?? Enumerable.Empty<EmailAddress>()).SequenceEqual(c2 ?? Enumerable.Empty<EmailAddress>()),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );

        modelBuilder.Entity<AdditionalDbUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<AdditionalDbUser>()
            .Property(e => e.Email)
            .HasConversion(emailConverter);
        modelBuilder.Entity<AdditionalDbUser>()
            .Property(e => e.RecoveryEmail)
            .HasConversion(nullableEmailConverter);
        modelBuilder.Entity<AdditionalDbUser>()
            .Property(e => e.Aliases)
            .HasConversion(emailListConverter, emailListValueComparer);


        modelBuilder.Entity<AdditionalDbGroup>()
            .HasIndex(g => g.Email)
            .IsUnique();
        modelBuilder.Entity<AdditionalDbGroup>()
            .Property(e => e.Email)
            .HasConversion(emailConverter);
        modelBuilder.Entity<AdditionalDbGroup>()
            .Property(e => e.Members)
            .HasConversion(emailListConverter, emailListValueComparer);
        modelBuilder.Entity<AdditionalDbGroup>()
            .Property(e => e.Aliases)
            .HasConversion(emailListConverter, emailListValueComparer);
    }
}