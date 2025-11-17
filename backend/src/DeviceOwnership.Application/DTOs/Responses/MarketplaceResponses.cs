using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Application.DTOs.Responses;

public record MarketplaceListingResponse(
    Guid Id,
    Guid DeviceId,
    Guid SellerId,
    string Title,
    string Description,
    decimal Price,
    string Currency,
    string? Condition,
    string Status,
    string? Category,
    string? Location,
    bool IsShippingAvailable,
    bool IsFeatured,
    int ViewCount,
    DateTime ListedAt,
    DateTime? SoldAt,
    DateTime? ExpiresAt,
    Guid? BuyerId
)
{
    public static MarketplaceListingResponse FromEntity(MarketplaceListing listing)
    {
        return new MarketplaceListingResponse(
            listing.Id,
            listing.DeviceId,
            listing.SellerId,
            listing.Title,
            listing.Description,
            listing.Price,
            listing.Currency,
            listing.Condition,
            listing.Status,
            listing.Category,
            listing.Location,
            listing.IsShippingAvailable,
            listing.IsFeatured,
            listing.ViewCount,
            listing.ListedAt,
            listing.SoldAt,
            listing.ExpiresAt,
            listing.BuyerId
        );
    }
}
