using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Application.DTOs.Requests;
using DeviceOwnership.Application.DTOs.Responses;
using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using System.Security.Claims;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MarketplaceController : ControllerBase
{
    private readonly IMarketplaceRepository _marketplaceRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<MarketplaceController> _logger;

    public MarketplaceController(
        IMarketplaceRepository marketplaceRepository,
        IDeviceRepository deviceRepository,
        ILogger<MarketplaceController> logger)
    {
        _marketplaceRepository = marketplaceRepository;
        _deviceRepository = deviceRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all active marketplace listings with optional filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetListings(
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? condition = null,
        [FromQuery] string? location = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var listings = await _marketplaceRepository.GetFilteredListingsAsync(
                category, minPrice, maxPrice, condition, location, cancellationToken);

            var response = listings.Select(MarketplaceListingResponse.FromEntity);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving marketplace listings");
            return StatusCode(500, new { message = "An error occurred while retrieving listings" });
        }
    }

    /// <summary>
    /// Get a specific marketplace listing by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListing(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var listing = await _marketplaceRepository.GetByIdAsync(id, cancellationToken);
            if (listing == null)
            {
                return NotFound(new { message = "Listing not found" });
            }

            // Increment view count
            listing.ViewCount++;
            listing.UpdatedAt = DateTime.UtcNow;
            await _marketplaceRepository.UpdateAsync(listing, cancellationToken);

            return Ok(MarketplaceListingResponse.FromEntity(listing));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listing {ListingId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the listing" });
        }
    }

    /// <summary>
    /// Get current user's marketplace listings
    /// </summary>
    [HttpGet("my-listings")]
    [Authorize]
    public async Task<IActionResult> GetMyListings(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var listings = await _marketplaceRepository.GetUserListingsAsync(userId, cancellationToken);
            var response = listings.Select(MarketplaceListingResponse.FromEntity);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user listings");
            return StatusCode(500, new { message = "An error occurred while retrieving your listings" });
        }
    }

    /// <summary>
    /// Create a new marketplace listing
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateListing([FromBody] CreateListingRequest request, CancellationToken cancellationToken)
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

            var listing = new MarketplaceListing
            {
                Id = Guid.NewGuid(),
                DeviceId = request.DeviceId,
                SellerId = userId,
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Currency = request.Currency,
                Condition = request.Condition,
                Category = request.Category,
                Location = request.Location,
                IsShippingAvailable = request.IsShippingAvailable,
                Status = "active",
                ViewCount = 0,
                ListedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _marketplaceRepository.AddAsync(listing, cancellationToken);

            return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, MarketplaceListingResponse.FromEntity(listing));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating marketplace listing");
            return StatusCode(500, new { message = "An error occurred while creating the listing" });
        }
    }

    /// <summary>
    /// Update an existing marketplace listing
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateListing(Guid id, [FromBody] UpdateListingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var listing = await _marketplaceRepository.GetByIdAsync(id, cancellationToken);
            if (listing == null)
            {
                return NotFound(new { message = "Listing not found" });
            }

            if (listing.SellerId != userId)
            {
                return Forbid();
            }

            // Update properties
            if (request.Title != null) listing.Title = request.Title;
            if (request.Description != null) listing.Description = request.Description;
            if (request.Price.HasValue) listing.Price = request.Price.Value;
            if (request.Condition != null) listing.Condition = request.Condition;
            if (request.Location != null) listing.Location = request.Location;
            if (request.IsShippingAvailable.HasValue) listing.IsShippingAvailable = request.IsShippingAvailable.Value;

            listing.UpdatedAt = DateTime.UtcNow;

            await _marketplaceRepository.UpdateAsync(listing, cancellationToken);

            return Ok(MarketplaceListingResponse.FromEntity(listing));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating listing {ListingId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the listing" });
        }
    }

    /// <summary>
    /// Delete a marketplace listing
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteListing(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var listing = await _marketplaceRepository.GetByIdAsync(id, cancellationToken);
            if (listing == null)
            {
                return NotFound(new { message = "Listing not found" });
            }

            if (listing.SellerId != userId)
            {
                return Forbid();
            }

            await _marketplaceRepository.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting listing {ListingId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the listing" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }
}
