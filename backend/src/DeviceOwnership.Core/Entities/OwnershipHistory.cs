namespace DeviceOwnership.Core.Entities;

public class OwnershipHistory
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid? FromUserId { get; set; } // null for initial registration
    public Guid ToUserId { get; set; }
    public string TransferMethod { get; set; } = string.Empty; // registered, transferred, marketplace
    public DateTime TransferredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string? VerificationDocumentUrl { get; set; }
    public string? IpAddress { get; set; }
    public string? Location { get; set; }

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
    public virtual User? FromUser { get; set; }
    public virtual User ToUser { get; set; } = null!;
}
