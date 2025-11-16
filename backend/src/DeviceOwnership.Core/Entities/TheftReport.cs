using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Core.Entities;

public class TheftReport
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public ReportType ReportType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? IncidentDate { get; set; }
    public string? IncidentLocation { get; set; }
    public string? IncidentCity { get; set; }
    public string? IncidentCountry { get; set; }
    public decimal? IncidentLatitude { get; set; }
    public decimal? IncidentLongitude { get; set; }
    public string? PoliceReferenceNumber { get; set; }
    public string? PoliceStation { get; set; }
    public string? PoliceOfficerName { get; set; }
    public string Status { get; set; } = "active"; // active, investigating, recovered, closed
    public bool IsPublic { get; set; } = true;
    public decimal? RewardAmount { get; set; }
    public string RewardCurrency { get; set; } = "GBP";
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RecoveredAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? SuspectDescription { get; set; }
    public string? WitnessInformation { get; set; }
    public string? AdditionalNotes { get; set; }

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<PoliceReport> PoliceReports { get; set; } = new List<PoliceReport>();
}
