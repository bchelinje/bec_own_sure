namespace DeviceOwnership.Core.Enums;

public enum TransactionType
{
    SubscriptionUpgrade,
    MarketplacePurchase,
    EscrowDeposit,
    EscrowRelease,
    Refund,
    Commission
}

public enum TransactionStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Refunded
}

public enum OrderStatus
{
    PendingPayment,
    PaymentReceived,
    EscrowHeld,
    Shipped,
    Delivered,
    Completed,
    Disputed,
    Cancelled,
    Refunded
}

public enum EscrowStatus
{
    Held,
    Released,
    Refunded,
    Disputed
}
