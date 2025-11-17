using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Core.Enums;
using System.Security.Claims;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create payment intent for subscription upgrade
    /// </summary>
    [HttpPost("subscription/create-intent")]
    [Authorize]
    public async Task<IActionResult> CreateSubscriptionPaymentIntent(
        [FromBody] CreateSubscriptionPaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var clientSecret = await _paymentService.CreateSubscriptionPaymentIntentAsync(
                userId,
                request.TargetTier,
                cancellationToken);

            return Ok(new { clientSecret });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subscription payment intent");
            return StatusCode(500, new { message = "An error occurred while creating payment" });
        }
    }

    /// <summary>
    /// Confirm subscription payment (webhook or manual)
    /// </summary>
    [HttpPost("subscription/confirm")]
    [Authorize]
    public async Task<IActionResult> ConfirmSubscriptionPayment(
        [FromBody] ConfirmPaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var success = await _paymentService.ConfirmSubscriptionPaymentAsync(
                request.PaymentIntentId,
                cancellationToken);

            if (success)
            {
                return Ok(new { message = "Subscription upgraded successfully" });
            }

            return BadRequest(new { message = "Payment confirmation failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming subscription payment");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get user's transaction history
    /// </summary>
    [HttpGet("transactions")]
    [Authorize]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var transactions = await _paymentService.GetUserTransactionsAsync(userId, cancellationToken);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }
}

public record CreateSubscriptionPaymentRequest(SubscriptionTier TargetTier);
public record ConfirmPaymentRequest(string PaymentIntentId);
