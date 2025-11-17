using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using System.Security.Claims;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IRepository<DevicePhoto> _photoRepository;
    private readonly IRepository<DeviceDocument> _documentRepository;
    private readonly ILogger<FilesController> _logger;
    private readonly string _uploadPath;

    public FilesController(
        IDeviceRepository deviceRepository,
        IRepository<DevicePhoto> photoRepository,
        IRepository<DeviceDocument> documentRepository,
        ILogger<FilesController> logger,
        IWebHostEnvironment environment)
    {
        _deviceRepository = deviceRepository;
        _photoRepository = photoRepository;
        _documentRepository = documentRepository;
        _logger = logger;
        _uploadPath = Path.Combine(environment.ContentRootPath, "uploads");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    /// <summary>
    /// Upload a photo for a device
    /// </summary>
    [HttpPost("devices/{deviceId}/photos")]
    [Authorize]
    public async Task<IActionResult> UploadDevicePhoto(
        Guid deviceId,
        IFormFile file,
        [FromForm] string? caption = null,
        [FromForm] bool isPrimary = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Verify device ownership
            var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken);
            if (device == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            if (device.UserId != userId)
            {
                return Forbid();
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file type. Only images are allowed." });
            }

            // Save file
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, "photos", fileName);
            var photoDirectory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(photoDirectory))
            {
                Directory.CreateDirectory(photoDirectory!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var photo = new DevicePhoto
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                PhotoUrl = $"/uploads/photos/{fileName}",
                Caption = caption,
                IsPrimary = isPrimary,
                UploadedAt = DateTime.UtcNow
            };

            await _photoRepository.AddAsync(photo, cancellationToken);

            return Ok(photo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for device {DeviceId}", deviceId);
            return StatusCode(500, new { message = "An error occurred while uploading the photo" });
        }
    }

    /// <summary>
    /// Upload a document for a device
    /// </summary>
    [HttpPost("devices/{deviceId}/documents")]
    [Authorize]
    public async Task<IActionResult> UploadDeviceDocument(
        Guid deviceId,
        IFormFile file,
        [FromForm] string documentType,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            // Verify device ownership
            var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken);
            if (device == null)
            {
                return NotFound(new { message = "Device not found" });
            }

            if (device.UserId != userId)
            {
                return Forbid();
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid file type." });
            }

            // Save file
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, "documents", fileName);
            var documentDirectory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(documentDirectory))
            {
                Directory.CreateDirectory(documentDirectory!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var document = new DeviceDocument
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                DocumentType = documentType,
                DocumentUrl = $"/uploads/documents/{fileName}",
                FileName = file.FileName,
                UploadedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document, cancellationToken);

            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for device {DeviceId}", deviceId);
            return StatusCode(500, new { message = "An error occurred while uploading the document" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }
}
