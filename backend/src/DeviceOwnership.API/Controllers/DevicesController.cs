using DeviceOwnership.Application.DTOs.Requests;
using DeviceOwnership.Application.DTOs.Responses;
using DeviceOwnership.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(
        IDeviceService deviceService,
        ILogger<DevicesController> logger)
    {
        _deviceService = deviceService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new device
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        try
        {
            // TODO: Get user ID from claims
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.NewGuid().ToString());

            var device = await _deviceService.RegisterDeviceAsync(
                userId,
                request.SerialNumber,
                request.Category,
                request.Brand,
                request.Model);

            var response = new DeviceResponse(
                device.Id,
                device.Category,
                device.Brand,
                device.Model,
                device.Description,
                device.Status.ToString(),
                device.IsStolen,
                device.IsLost,
                device.VerificationCode,
                device.QRCodeUrl,
                device.RegisteredAt,
                null
            );

            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device");
            return StatusCode(500, new { error = "An error occurred while registering the device" });
        }
    }

    /// <summary>
    /// Get all user's devices
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDevices()
    {
        try
        {
            // TODO: Get user ID from claims
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.NewGuid().ToString());

            var devices = await _deviceService.GetUserDevicesAsync(userId);

            var response = devices.Select(d => new DeviceResponse(
                d.Id,
                d.Category,
                d.Brand,
                d.Model,
                d.Description,
                d.Status.ToString(),
                d.IsStolen,
                d.IsLost,
                d.VerificationCode,
                d.QRCodeUrl,
                d.RegisteredAt,
                d.Photos?.FirstOrDefault(p => p.IsPrimary)?.PhotoUrl
            ));

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user devices");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Get device by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDevice(Guid id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound(new { error = "Device not found" });
            }

            var response = new DeviceResponse(
                device.Id,
                device.Category,
                device.Brand,
                device.Model,
                device.Description,
                device.Status.ToString(),
                device.IsStolen,
                device.IsLost,
                device.VerificationCode,
                device.QRCodeUrl,
                device.RegisteredAt,
                device.Photos?.FirstOrDefault(p => p.IsPrimary)?.PhotoUrl
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Check if a serial number is stolen/lost (Public endpoint)
    /// </summary>
    [HttpGet("check/{serialNumber}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckSerialNumber(string serialNumber)
    {
        try
        {
            var device = await _deviceService.CheckSerialNumberAsync(serialNumber);

            if (device == null)
            {
                return Ok(new
                {
                    status = "not_found",
                    message = "This device is not registered in our system"
                });
            }

            if (device.IsStolen)
            {
                return Ok(new
                {
                    status = "stolen",
                    isStolen = true,
                    message = "⚠️ This device has been reported as STOLEN. Contact authorities immediately.",
                    reportedAt = device.TheftReports?.FirstOrDefault()?.ReportedAt
                });
            }

            if (device.IsLost)
            {
                return Ok(new
                {
                    status = "lost",
                    isLost = true,
                    message = "This device has been reported as lost. Contact the owner.",
                    reportedAt = device.TheftReports?.FirstOrDefault()?.ReportedAt
                });
            }

            return Ok(new
            {
                status = "registered",
                message = "This device is registered to a verified owner. Verify ownership transfer before purchase.",
                registeredAt = device.RegisteredAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking serial number");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    /// <summary>
    /// Delete a device (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        try
        {
            await _deviceService.DeleteDeviceAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device");
            return StatusCode(500, new { error = "An error occurred" });
        }
    }
}
