using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Application.DTOs.Responses;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType,
    UserResponse User
);

public record UserResponse(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string Role,
    string SubscriptionTier,
    bool IsEmailVerified,
    bool IsPhoneVerified,
    string? ProfilePhotoUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static UserResponse FromEntity(User user)
    {
        return new UserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.SubscriptionTier.ToString(),
            user.IsEmailVerified,
            user.IsPhoneVerified,
            user.ProfilePhotoUrl,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
