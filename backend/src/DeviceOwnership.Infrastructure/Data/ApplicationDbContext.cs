using DeviceOwnership.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceOwnership.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Entity DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<DevicePhoto> DevicePhotos { get; set; } = null!;
    public DbSet<DeviceDocument> DeviceDocuments { get; set; } = null!;
    public DbSet<OwnershipHistory> OwnershipHistory { get; set; } = null!;
    public DbSet<TheftReport> TheftReports { get; set; } = null!;
    public DbSet<MarketplaceListing> MarketplaceListings { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<PoliceProfile> PoliceProfiles { get; set; } = null!;
    public DbSet<BusinessProfile> BusinessProfiles { get; set; } = null!;
    public DbSet<PoliceReport> PoliceReports { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Escrow> Escrows { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure enum to string conversion
        modelBuilder.Entity<User>()
            .Property(u => u.SubscriptionTier)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Device>()
            .Property(d => d.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TheftReport>()
            .Property(t => t.ReportType)
            .HasConversion<string>();

        // Indexes for performance
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Device>()
            .HasIndex(d => d.SerialNumberHash)
            .IsUnique();

        modelBuilder.Entity<Device>()
            .HasIndex(d => d.UserId);

        modelBuilder.Entity<Device>()
            .HasIndex(d => d.VerificationCode);

        modelBuilder.Entity<TheftReport>()
            .HasIndex(t => t.DeviceId);

        modelBuilder.Entity<MarketplaceListing>()
            .HasIndex(m => m.DeviceId);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead });

        // Configure relationships
        modelBuilder.Entity<Device>()
            .HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TheftReport>()
            .HasOne(t => t.Device)
            .WithMany(d => d.TheftReports)
            .HasForeignKey(t => t.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TheftReport>()
            .HasOne(t => t.User)
            .WithMany(u => u.TheftReports)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OwnershipHistory>()
            .HasOne(o => o.Device)
            .WithMany(d => d.OwnershipHistory)
            .HasForeignKey(o => o.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OwnershipHistory>()
            .HasOne(o => o.ToUser)
            .WithMany()
            .HasForeignKey(o => o.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OwnershipHistory>()
            .HasOne(o => o.FromUser)
            .WithMany()
            .HasForeignKey(o => o.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MarketplaceListing>()
            .HasOne(m => m.Device)
            .WithOne(d => d.MarketplaceListing)
            .HasForeignKey<MarketplaceListing>(m => m.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MarketplaceListing>()
            .HasOne(m => m.Seller)
            .WithMany()
            .HasForeignKey(m => m.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MarketplaceListing>()
            .HasOne(m => m.Buyer)
            .WithMany()
            .HasForeignKey(m => m.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithOne(u => u.Subscription)
            .HasForeignKey<Subscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PoliceProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.PoliceProfile)
            .HasForeignKey<PoliceProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BusinessProfile>()
            .HasOne(b => b.User)
            .WithOne(u => u.BusinessProfile)
            .HasForeignKey<BusinessProfile>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment entity configurations
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Listing)
            .WithMany()
            .HasForeignKey(o => o.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Escrow>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Escrow>()
            .HasOne(e => e.Order)
            .WithOne(o => o.Escrow)
            .HasForeignKey<Escrow>(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user && entry.State == EntityState.Modified)
            {
                user.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Device device && entry.State == EntityState.Modified)
            {
                device.LastUpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
