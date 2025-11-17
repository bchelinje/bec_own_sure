using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public AuthService(
        IUserRepository userRepository,
        string jwtSecret = "dev-jwt-secret-key-min-32-chars-long!",
        string jwtIssuer = "DeviceOwnershipAPI",
        int accessTokenExpirationMinutes = 60,
        int refreshTokenExpirationDays = 7)
    {
        _userRepository = userRepository;
        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
        _accessTokenExpirationMinutes = accessTokenExpirationMinutes;
        _refreshTokenExpirationDays = refreshTokenExpirationDays;
    }

    public async Task<(User User, string AccessToken, string RefreshToken)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token (in production, store in database)
        user.UpdatedAt = DateTime.UtcNow;

        return (user, accessToken, refreshToken);
    }

    public async Task<(User User, string AccessToken, string RefreshToken)> RegisterAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Role = UserRole.User,
            SubscriptionTier = SubscriptionTier.Free,
            IsEmailVerified = false,
            IsPhoneVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        return (user, accessToken, refreshToken);
    }

    public async Task<(User User, string AccessToken, string RefreshToken)> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // In production, validate refresh token from database
        // For now, we'll just generate new tokens
        throw new NotImplementedException("Refresh token validation not yet implemented");
    }

    public string HashPassword(string password)
    {
        // Using BCrypt-like simple implementation (in production, use BCrypt.Net or ASP.NET Core Identity)
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("subscription_tier", user.SubscriptionTier.ToString())
        };

        if (!string.IsNullOrEmpty(user.FirstName))
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));

        if (!string.IsNullOrEmpty(user.LastName))
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            Issuer = _jwtIssuer,
            Audience = _jwtIssuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
