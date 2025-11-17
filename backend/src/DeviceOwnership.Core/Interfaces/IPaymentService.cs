using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IPaymentService
{
    // Subscription payments
    Task<string> CreateSubscriptionPaymentIntentAsync(Guid userId, SubscriptionTier targetTier, CancellationToken cancellationToken = default);
    Task<bool> ConfirmSubscriptionPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);

    // Marketplace payments
    Task<(Order Order, string PaymentIntentId)> CreateMarketplacePurchaseAsync(Guid buyerId, Guid listingId, string shippingAddress, CancellationToken cancellationToken = default);
    Task<bool> ConfirmMarketplacePurchaseAsync(string paymentIntentId, CancellationToken cancellationToken = default);

    // Escrow management
    Task<bool> ReleaseEscrowAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> RefundEscrowAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
    Task ProcessAutoReleaseAsync(CancellationToken cancellationToken = default);

    // Transaction queries
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
