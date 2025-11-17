using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Application.DTOs.Requests;
using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using System.Security.Claims;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IRepository<TheftReport> _reportRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IRepository<TheftReport> reportRepository,
        IDeviceRepository deviceRepository,
        ILogger<ReportsController> logger)
    {
        _reportRepository = reportRepository;
        _deviceRepository = deviceRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all theft reports for the current user
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserReports(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var reports = await _reportRepository.GetAllAsync(cancellationToken);
            var userReports = reports.Where(r => r.UserId == userId);

            return Ok(userReports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user reports");
            return StatusCode(500, new { message = "An error occurred while retrieving reports" });
        }
    }

    /// <summary>
    /// Get a specific theft report by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetReport(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
            if (report == null)
            {
                return NotFound(new { message = "Report not found" });
            }

            if (report.UserId != userId)
            {
                return Forbid();
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report {ReportId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the report" });
        }
    }

    /// <summary>
    /// Create a new theft report
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReport([FromBody] CreateTheftReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Verify device ownership
            var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
            if (device == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            if (device.UserId != userId)
            {
                return Forbid();
            }

            var report = new TheftReport
            {
                Id = Guid.NewGuid(),
                DeviceId = request.DeviceId,
                UserId = userId,
                ReportType = request.ReportType,
                IncidentDate = request.IncidentDate,
                Location = request.Location,
                PoliceReportNumber = request.PoliceReportNumber,
                Description = request.Description,
                Status = "active",
                ReportedAt = DateTime.UtcNow
            };

            await _reportRepository.AddAsync(report, cancellationToken);

            // Update device status to Stolen if report type is Stolen
            if (request.ReportType == Core.Enums.ReportType.Stolen)
            {
                device.Status = Core.Enums.DeviceStatus.Stolen;
                device.LastUpdatedAt = DateTime.UtcNow;
                await _deviceRepository.UpdateAsync(device, cancellationToken);
            }

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating theft report");
            return StatusCode(500, new { message = "An error occurred while creating the report" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }
}
