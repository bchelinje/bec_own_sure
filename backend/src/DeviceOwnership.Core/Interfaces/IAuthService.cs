using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IAuthService
{
    Task<(User User, string AccessToken, string RefreshToken)> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(User User, string AccessToken, string RefreshToken)> RegisterAsync(string email, string password, string? firstName, string? lastName, string? phoneNumber, CancellationToken cancellationToken = default);
    Task<(User User, string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
