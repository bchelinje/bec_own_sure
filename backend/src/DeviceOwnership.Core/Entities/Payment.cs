using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Core.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public string? Description { get; set; }
    public string? Metadata { get; set; } // JSON for additional data
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}

public class Order
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal SellerAmount { get; set; }
    public string Currency { get; set; } = "GBP";
    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
    public Guid? TransactionId { get; set; }
    public Guid? EscrowId { get; set; }
    public string? ShippingAddress { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual MarketplaceListing Listing { get; set; } = null!;
    public virtual User Buyer { get; set; } = null!;
    public virtual User Seller { get; set; } = null!;
    public virtual Transaction? Transaction { get; set; }
    public virtual Escrow? Escrow { get; set; }
}

public class Escrow
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
    public EscrowStatus Status { get; set; } = EscrowStatus.Held;
    public string? StripeAccountId { get; set; }
    public DateTime HeldAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReleasedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? ReleaseReason { get; set; }
    public int AutoReleaseDays { get; set; } = 14; // Auto-release after 14 days
    public DateTime AutoReleaseDate { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}
