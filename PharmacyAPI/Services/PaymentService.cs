using Microsoft.EntityFrameworkCore;
using PharmacyAPI.Data;
using PharmacyAPI.DTOs;
using PharmacyAPI.Models;

namespace PharmacyAPI.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto?> ProcessPaymentAsync(int orderId, int userId, ProcessPaymentDto dto);
        Task<PaymentResponseDto?> GetPaymentByOrderIdAsync(int orderId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentResponseDto?> ProcessPaymentAsync(int orderId, int userId, ProcessPaymentDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null) return null;

            var payment = order.Payment;
            if (payment == null) return null;

            // Simulate payment processing
            payment.Method = dto.Method;
            payment.TransactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            payment.Status = "Success";
            payment.PaidAt = DateTime.UtcNow;

            // Update order status to Confirmed
            order.Status = "Confirmed";
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                Method = payment.Method,
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt
            };
        }

        public async Task<PaymentResponseDto?> GetPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null) return null;

            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                Method = payment.Method,
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt
            };
        }
    }
}
