namespace DeviceOwnership.Application.DTOs.Responses;

public record DeviceResponse(
    Guid Id,
    string Category,
    string? Brand,
    string? Model,
    string? Description,
    string Status,
    bool IsStolen,
    bool IsLost,
    string? VerificationCode,
    string? QRCodeUrl,
    DateTime RegisteredAt,
    string? PrimaryPhotoUrl
);
