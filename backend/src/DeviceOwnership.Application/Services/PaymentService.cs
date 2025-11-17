using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Enums;
using DeviceOwnership.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Text.Json;

namespace DeviceOwnership.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Escrow> _escrowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMarketplaceRepository _marketplaceRepository;
    private readonly IConfiguration _configuration;
    private readonly decimal _platformFeePercentage = 0.05m; // 5% platform fee

    public PaymentService(
        IRepository<Transaction> transactionRepository,
        IRepository<Order> orderRepository,
        IRepository<Escrow> escrowRepository,
        IUserRepository userRepository,
        IMarketplaceRepository marketplaceRepository,
        IConfiguration configuration)
    {
        _transactionRepository = transactionRepository;
        _orderRepository = orderRepository;
        _escrowRepository = escrowRepository;
        _userRepository = userRepository;
        _marketplaceRepository = marketplaceRepository;
        _configuration = configuration;

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"] ?? "sk_test_dummy";
    }

    public async Task<string> CreateSubscriptionPaymentIntentAsync(Guid userId, SubscriptionTier targetTier, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var amount = GetSubscriptionPrice(targetTier);

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Convert to cents
            Currency = "gbp",
            Description = $"Subscription upgrade to {targetTier}",
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "type", "subscription" },
                { "tier", targetTier.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

        // Create transaction record
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = TransactionType.SubscriptionUpgrade,
            Amount = amount,
            Currency = "GBP",
            Status = TransactionStatus.Pending,
            StripePaymentIntentId = paymentIntent.Id,
            Description = $"Upgrade to {targetTier} subscription",
            Metadata = JsonSerializer.Serialize(new { targetTier }),
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        return paymentIntent.ClientSecret!;
    }

    public async Task<bool> ConfirmSubscriptionPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var transaction = (await _transactionRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(t => t.StripePaymentIntentId == paymentIntentId);

        if (transaction == null)
        {
            return false;
        }

        var service = new PaymentIntentService();
        var paymentIntent = await service.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent.Status == "succeeded")
        {
            transaction.Status = TransactionStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;
            transaction.StripeChargeId = paymentIntent.LatestChargeId;

            // Update user subscription
            var user = await _userRepository.GetByIdAsync(transaction.UserId, cancellationToken);
            if (user != null)
            {
                var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(transaction.Metadata ?? "{}");
                if (metadata != null && metadata.TryGetValue("targetTier", out var tierString))
                {
                    user.SubscriptionTier = Enum.Parse<SubscriptionTier>(tierString);
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user, cancellationToken);
                }
            }

            await _transactionRepository.UpdateAsync(transaction, cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<(Order Order, string PaymentIntentId)> CreateMarketplacePurchaseAsync(
        Guid buyerId,
        Guid listingId,
        string shippingAddress,
        CancellationToken cancellationToken = default)
    {
        var listing = await _marketplaceRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing == null || listing.Status != "active")
        {
            throw new InvalidOperationException("Listing not available");
        }

        if (listing.SellerId == buyerId)
        {
            throw new InvalidOperationException("Cannot purchase your own listing");
        }

        // Calculate amounts
        var totalAmount = listing.Price;
        var platformFee = totalAmount * _platformFeePercentage;
        var sellerAmount = totalAmount - platformFee;

        // Create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            BuyerId = buyerId,
            SellerId = listing.SellerId,
            TotalAmount = totalAmount,
            PlatformFee = platformFee,
            SellerAmount = sellerAmount,
            Currency = listing.Currency,
            Status = OrderStatus.PendingPayment,
            ShippingAddress = shippingAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        // Create Stripe payment intent
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(totalAmount * 100), // Convert to cents
            Currency = listing.Currency.ToLower(),
            Description = $"Purchase: {listing.Title}",
            Metadata = new Dictionary<string, string>
            {
                { "orderId", order.Id.ToString() },
                { "buyerId", buyerId.ToString() },
                { "sellerId", listing.SellerId.ToString() },
                { "listingId", listingId.ToString() }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

        // Create transaction
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = buyerId,
            Type = TransactionType.MarketplacePurchase,
            Amount = totalAmount,
            Currency = listing.Currency,
            Status = TransactionStatus.Pending,
            StripePaymentIntentId = paymentIntent.Id,
            Description = $"Purchase: {listing.Title}",
            Metadata = JsonSerializer.Serialize(new { orderId = order.Id }),
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        order.TransactionId = transaction.Id;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return (order, paymentIntent.ClientSecret!);
    }

    public async Task<bool> ConfirmMarketplacePurchaseAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var transaction = (await _transactionRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(t => t.StripePaymentIntentId == paymentIntentId);

        if (transaction == null)
        {
            return false;
        }

        var service = new PaymentIntentService();
        var paymentIntent = await service.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent.Status == "succeeded")
        {
            transaction.Status = TransactionStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;
            transaction.StripeChargeId = paymentIntent.LatestChargeId;
            await _transactionRepository.UpdateAsync(transaction, cancellationToken);

            // Get order and create escrow
            var metadata = JsonSerializer.Deserialize<Dictionary<string, Guid>>(transaction.Metadata ?? "{}");
            if (metadata != null && metadata.TryGetValue("orderId", out var orderId))
            {
                var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
                if (order != null)
                {
                    order.Status = OrderStatus.EscrowHeld;
                    await _orderRepository.UpdateAsync(order, cancellationToken);

                    // Create escrow
                    var escrow = new Escrow
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        Amount = order.SellerAmount,
                        Currency = order.Currency,
                        Status = EscrowStatus.Held,
                        HeldAt = DateTime.UtcNow,
                        AutoReleaseDate = DateTime.UtcNow.AddDays(14)
                    };

                    await _escrowRepository.AddAsync(escrow, cancellationToken);

                    order.EscrowId = escrow.Id;
                    await _orderRepository.UpdateAsync(order, cancellationToken);

                    // Update listing status
                    var listing = await _marketplaceRepository.GetByIdAsync(order.ListingId, cancellationToken);
                    if (listing != null)
                    {
                        listing.Status = "sold";
                        listing.BuyerId = order.BuyerId;
                        listing.SoldAt = DateTime.UtcNow;
                        await _marketplaceRepository.UpdateAsync(listing, cancellationToken);
                    }
                }
            }

            return true;
        }

        return false;
    }

    public async Task<bool> ReleaseEscrowAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null || order.EscrowId == null)
        {
            return false;
        }

        var escrow = await _escrowRepository.GetByIdAsync(order.EscrowId.Value, cancellationToken);
        if (escrow == null || escrow.Status != EscrowStatus.Held)
        {
            return false;
        }

        // Release funds to seller (in production, transfer to seller's Stripe account)
        escrow.Status = EscrowStatus.Released;
        escrow.ReleasedAt = DateTime.UtcNow;
        escrow.ReleaseReason = "Buyer confirmed delivery";
        await _escrowRepository.UpdateAsync(escrow, cancellationToken);

        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTime.UtcNow;
        order.ConfirmedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        // Create seller payout transaction
        var payoutTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = order.SellerId,
            Type = TransactionType.EscrowRelease,
            Amount = order.SellerAmount,
            Currency = order.Currency,
            Status = TransactionStatus.Completed,
            Description = $"Payment received for order #{order.Id}",
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(payoutTransaction, cancellationToken);

        return true;
    }

    public async Task<bool> RefundEscrowAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null || order.EscrowId == null || order.TransactionId == null)
        {
            return false;
        }

        var escrow = await _escrowRepository.GetByIdAsync(order.EscrowId.Value, cancellationToken);
        if (escrow == null || escrow.Status != EscrowStatus.Held)
        {
            return false;
        }

        var transaction = await _transactionRepository.GetByIdAsync(order.TransactionId.Value, cancellationToken);
        if (transaction == null || string.IsNullOrEmpty(transaction.StripeChargeId))
        {
            return false;
        }

        // Process Stripe refund
        try
        {
            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                Charge = transaction.StripeChargeId,
                Reason = "requested_by_customer"
            };

            await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);

            // Update escrow
            escrow.Status = EscrowStatus.Refunded;
            escrow.RefundedAt = DateTime.UtcNow;
            await _escrowRepository.UpdateAsync(escrow, cancellationToken);

            // Update order
            order.Status = OrderStatus.Refunded;
            order.Notes = reason;
            await _orderRepository.UpdateAsync(order, cancellationToken);

            // Create refund transaction
            var refundTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = order.BuyerId,
                Type = TransactionType.Refund,
                Amount = order.TotalAmount,
                Currency = order.Currency,
                Status = TransactionStatus.Completed,
                Description = $"Refund for order #{order.Id}: {reason}",
                CompletedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(refundTransaction, cancellationToken);

            // Reactivate listing
            var listing = await _marketplaceRepository.GetByIdAsync(order.ListingId, cancellationToken);
            if (listing != null)
            {
                listing.Status = "active";
                listing.BuyerId = null;
                listing.SoldAt = null;
                await _marketplaceRepository.UpdateAsync(listing, cancellationToken);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task ProcessAutoReleaseAsync(CancellationToken cancellationToken = default)
    {
        var allEscrows = await _escrowRepository.GetAllAsync(cancellationToken);
        var expiredEscrows = allEscrows
            .Where(e => e.Status == EscrowStatus.Held && e.AutoReleaseDate <= DateTime.UtcNow);

        foreach (var escrow in expiredEscrows)
        {
            await ReleaseEscrowAsync(escrow.OrderId, cancellationToken);
        }
    }

    public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var allTransactions = await _transactionRepository.GetAllAsync(cancellationToken);
        return allTransactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt);
    }

    private decimal GetSubscriptionPrice(SubscriptionTier tier)
    {
        return tier switch
        {
            SubscriptionTier.Basic => 4.99m,
            SubscriptionTier.Premium => 9.99m,
            SubscriptionTier.Enterprise => 49.99m,
            _ => 0m
        };
    }
}
