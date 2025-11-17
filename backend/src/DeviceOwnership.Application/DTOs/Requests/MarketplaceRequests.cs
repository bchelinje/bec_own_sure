namespace DeviceOwnership.Application.DTOs.Requests;

public record CreateListingRequest(
    Guid DeviceId,
    string Title,
    string Description,
    decimal Price,
    string Currency,
    string? Condition,
    string? Category,
    string? Location,
    bool IsShippingAvailable,
    DateTime? ExpiresAt
);

public record UpdateListingRequest(
    string? Title,
    string? Description,
    decimal? Price,
    string? Condition,
    string? Location,
    bool? IsShippingAvailable
);
