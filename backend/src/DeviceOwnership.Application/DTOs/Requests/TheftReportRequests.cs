using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.Application.DTOs.Requests;

public record CreateTheftReportRequest(
    Guid DeviceId,
    ReportType ReportType,
    DateTime IncidentDate,
    string? Location,
    string? PoliceReportNumber,
    string Description
);
