namespace DeviceOwnership.Core.Entities;

public class PoliceProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BadgeNumber { get; set; } = string.Empty;
    public string? Rank { get; set; }
    public string? Department { get; set; }
    public string? PoliceStation { get; set; }
    public string? StationAddress { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? OfficialEmail { get; set; }
    public string? OfficialPhone { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public Guid? VerifiedByAdminId { get; set; }
    public string? VerificationNotes { get; set; }
    public string? IdDocumentUrl { get; set; }
    public string? BadgePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual User? VerifiedByAdmin { get; set; }
}
