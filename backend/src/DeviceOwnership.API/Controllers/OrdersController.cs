using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Core.Entities;
using System.Security.Claims;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IRepository<Order> _orderRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IPaymentService paymentService,
        IRepository<Order> orderRepository,
        ILogger<OrdersController> logger)
    {
        _paymentService = paymentService;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Create order and payment intent for marketplace purchase
    /// </summary>
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var (order, clientSecret) = await _paymentService.CreateMarketplacePurchaseAsync(
                userId,
                request.ListingId,
                request.ShippingAddress,
                cancellationToken);

            return Ok(new
            {
                orderId = order.Id,
                clientSecret,
                amount = order.TotalAmount,
                currency = order.Currency
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { message = "An error occurred while creating order" });
        }
    }

    /// <summary>
    /// Confirm order payment
    /// </summary>
    [HttpPost("{orderId}/confirm-payment")]
    [Authorize]
    public async Task<IActionResult> ConfirmPayment(
        Guid orderId,
        [FromBody] ConfirmPaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var success = await _paymentService.ConfirmMarketplacePurchaseAsync(
                request.PaymentIntentId,
                cancellationToken);

            if (success)
            {
                return Ok(new { message = "Payment confirmed. Funds held in escrow." });
            }

            return BadRequest(new { message = "Payment confirmation failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming payment for order {OrderId}", orderId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get user's orders (as buyer or seller)
    /// </summary>
    [HttpGet("my-orders")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var allOrders = await _orderRepository.GetAllAsync(cancellationToken);
            var myOrders = allOrders.Where(o => o.BuyerId == userId || o.SellerId == userId)
                .OrderByDescending(o => o.CreatedAt);

            return Ok(myOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Get specific order details
    /// </summary>
    [HttpGet("{orderId}")]
    [Authorize]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Only buyer or seller can view order
            if (order.BuyerId != userId && order.SellerId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Mark order as shipped (seller only)
    /// </summary>
    [HttpPost("{orderId}/ship")]
    [Authorize]
    public async Task<IActionResult> ShipOrder(
        Guid orderId,
        [FromBody] ShipOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            if (order.SellerId != userId)
            {
                return Forbid();
            }

            order.Status = Core.Enums.OrderStatus.Shipped;
            order.TrackingNumber = request.TrackingNumber;
            order.ShippedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order, cancellationToken);

            return Ok(new { message = "Order marked as shipped" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order {OrderId}", orderId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Confirm delivery and release escrow (buyer only)
    /// </summary>
    [HttpPost("{orderId}/confirm-delivery")]
    [Authorize]
    public async Task<IActionResult> ConfirmDelivery(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            if (order.BuyerId != userId)
            {
                return Forbid();
            }

            var success = await _paymentService.ReleaseEscrowAsync(orderId, cancellationToken);

            if (success)
            {
                return Ok(new { message = "Delivery confirmed. Payment released to seller." });
            }

            return BadRequest(new { message = "Failed to release payment" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming delivery for order {OrderId}", orderId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    /// <summary>
    /// Request refund (buyer only)
    /// </summary>
    [HttpPost("{orderId}/refund")]
    [Authorize]
    public async Task<IActionResult> RequestRefund(
        Guid orderId,
        [FromBody] RefundRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            if (order.BuyerId != userId)
            {
                return Forbid();
            }

            var success = await _paymentService.RefundEscrowAsync(orderId, request.Reason, cancellationToken);

            if (success)
            {
                return Ok(new { message = "Refund processed successfully" });
            }

            return BadRequest(new { message = "Failed to process refund" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for order {OrderId}", orderId);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }
}

public record CreateOrderRequest(Guid ListingId, string ShippingAddress);
public record ConfirmPaymentRequest(string PaymentIntentId);
public record ShipOrderRequest(string TrackingNumber);
public record RefundRequest(string Reason);
