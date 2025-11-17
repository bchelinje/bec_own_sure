namespace DeviceOwnership.Application.DTOs.Requests;

public record RegisterDeviceRequest(
    string SerialNumber,
    string Category,
    string? Brand,
    string? Model,
    string? Description,
    string? Color,
    DateTime? PurchaseDate,
    decimal? PurchasePrice,
    string? PurchaseCurrency
);
