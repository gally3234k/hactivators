using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyAPI.DTOs;
using PharmacyAPI.Services;

namespace PharmacyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId() => int.Parse(User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        /// <summary>
        /// Place a new order (Customer)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(GetUserId(), dto);
                return StatusCode(201, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get logged-in user's order history
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetUserOrdersAsync(GetUserId());
            return Ok(orders);
        }

        /// <summary>
        /// Get a single order by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            // Customers can only see their own
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Customer" && order.UserId != GetUserId())
                return Forbid();

            return Ok(order);
        }

        /// <summary>
        /// Get all orders (Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] DateTime? fromDate)
        {
            var orders = await _orderService.GetAllOrdersAsync(status, fromDate);
            return Ok(orders);
        }

        /// <summary>
        /// Update order status (Admin)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(dto.Status))
                return BadRequest(new { message = $"Invalid status. Valid: {string.Join(", ", validStatuses)}" });

            var order = await _orderService.UpdateStatusAsync(id, dto.Status);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            return Ok(order);
        }
        /// <summary>
        /// Cancel an order (Customer)
        /// </summary>
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            if (order.UserId != GetUserId())
                return Forbid();

            if (order.Status != "Pending" && order.Status != "Processing")
                return BadRequest(new { message = "Order cannot be cancelled at this stage." });

            var updatedOrder = await _orderService.UpdateStatusAsync(id, "Cancelled");
            return Ok(updatedOrder);
        }
    }
}
