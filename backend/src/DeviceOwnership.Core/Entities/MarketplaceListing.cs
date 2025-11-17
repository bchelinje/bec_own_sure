namespace DeviceOwnership.Core.Entities;

public class MarketplaceListing
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid SellerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "GBP";
    public string? Condition { get; set; } // new, like_new, good, fair, poor
    public string Status { get; set; } = "active"; // active, sold, expired, removed
    public string? Category { get; set; }
    public string? Location { get; set; }
    public bool IsShippingAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime ListedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SoldAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? BuyerId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
    public virtual User Seller { get; set; } = null!;
    public virtual User? Buyer { get; set; }
}
