namespace DeviceOwnership.Core.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } = string.Empty; // free, premium_monthly, premium_yearly, business
    public string Status { get; set; } = "active"; // active, cancelled, expired, past_due
    public string? BillingInterval { get; set; } // monthly, yearly
    public decimal? Amount { get; set; }
    public string Currency { get; set; } = "GBP";
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public DateTime? TrialStartedAt { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripePriceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual User User { get; set; } = null!;
}
