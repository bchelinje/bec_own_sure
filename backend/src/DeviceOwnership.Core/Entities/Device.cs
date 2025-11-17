using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Core.Entities;

public class Device
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SerialNumberHash { get; set; } = string.Empty;
    public string SerialNumberEncrypted { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string PurchaseCurrency { get; set; } = "GBP";
    public DateTime? WarrantyExpiryDate { get; set; }
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;
    public bool IsStolen { get; set; }
    public bool IsLost { get; set; }
    public string? VerificationCode { get; set; }
    public string? QRCodeUrl { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public string? Metadata { get; set; } // JSON

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<DevicePhoto> Photos { get; set; } = new List<DevicePhoto>();
    public virtual ICollection<DeviceDocument> Documents { get; set; } = new List<DeviceDocument>();
    public virtual ICollection<OwnershipHistory> OwnershipHistory { get; set; } = new List<OwnershipHistory>();
    public virtual ICollection<TheftReport> TheftReports { get; set; } = new List<TheftReport>();
    public virtual MarketplaceListing? MarketplaceListing { get; set; }
}
