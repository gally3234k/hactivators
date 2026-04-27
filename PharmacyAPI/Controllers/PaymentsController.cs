using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyAPI.DTOs;
using PharmacyAPI.Services;

namespace PharmacyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId() => int.Parse(User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        /// <summary>
        /// Process payment for an order (Customer)
        /// </summary>
        [HttpPost("process/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ProcessPayment(int orderId, [FromBody] ProcessPaymentDto dto)
        {
            var payment = await _paymentService.ProcessPaymentAsync(orderId, GetUserId(), dto);
            if (payment == null)
                return NotFound(new { message = "Order not found or not authorized." });

            return Ok(payment);
        }

        /// <summary>
        /// Get payment details for an order
        /// </summary>
        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetPayment(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment == null)
                return NotFound(new { message = "Payment not found." });

            return Ok(payment);
        }
    }
}
