namespace DeviceOwnership.Core.Entities;

public class PoliceReport
{
    public Guid Id { get; set; }
    public Guid TheftReportId { get; set; }
    public Guid PoliceUserId { get; set; }
    public string? OfficerBadgeNumber { get; set; }
    public string? OfficerName { get; set; }
    public string? PoliceStation { get; set; }
    public string? CaseNumber { get; set; }
    public string? Notes { get; set; }
    public string? RecoveryStatus { get; set; } // under_investigation, evidence_collected, recovered, closed
    public string? EvidenceUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual TheftReport TheftReport { get; set; } = null!;
    public virtual User PoliceUser { get; set; } = null!;
}
