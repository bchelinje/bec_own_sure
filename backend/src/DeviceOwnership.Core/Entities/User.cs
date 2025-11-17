using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
    public virtual ICollection<TheftReport> TheftReports { get; set; } = new List<TheftReport>();
    public virtual ICollection<MarketplaceListing> Listings { get; set; } = new List<MarketplaceListing>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual Subscription? Subscription { get; set; }
    public virtual PoliceProfile? PoliceProfile { get; set; }
    public virtual BusinessProfile? BusinessProfile { get; set; }
}
