namespace DeviceOwnership.Core.Entities;

public class BusinessProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyRegistrationNumber { get; set; }
    public string? VatNumber { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationDocumentUrl { get; set; }
    public string? ApiKeyHash { get; set; }
    public DateTime? ApiKeyCreatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual User User { get; set; } = null!;
}
