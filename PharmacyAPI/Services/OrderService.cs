using Microsoft.EntityFrameworkCore;
using PharmacyAPI.Data;
using PharmacyAPI.DTOs;
using PharmacyAPI.Helpers;
using PharmacyAPI.Models;
using PharmacyAPI.Repositories;

namespace PharmacyAPI.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int userId, CreateOrderDto dto);
        Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status, DateTime? fromDate);
        Task<OrderResponseDto?> UpdateStatusAsync(int id, string status);
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Medicine> _medicineRepo;
        private readonly IRepository<Prescription> _prescriptionRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IEmailHelper _emailHelper;

        public OrderService(
            AppDbContext context,
            IRepository<Order> orderRepo,
            IRepository<Medicine> medicineRepo,
            IRepository<Prescription> prescriptionRepo,
            IRepository<User> userRepo,
            IEmailHelper emailHelper)
        {
            _context = context;
            _orderRepo = orderRepo;
            _medicineRepo = medicineRepo;
            _prescriptionRepo = prescriptionRepo;
            _userRepo = userRepo;
            _emailHelper = emailHelper;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, CreateOrderDto dto)
        {
            // Use execution strategy to support retries with user-initiated transactions
            var strategy = _context.Database.CreateExecutionStrategy();

            Order? savedOrder = null;

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var user = await _userRepo.GetByIdAsync(userId);
                    if (user == null) throw new InvalidOperationException("User not found.");

                    // Validate prescription if needed
                    var orderItems = new List<OrderItem>();
                    decimal totalAmount = 0;
                    bool needsPrescription = false;

                    foreach (var item in dto.Items)
                    {
                        var medicine = await _medicineRepo.GetByIdAsync(item.MedicineId);
                        if (medicine == null)
                            throw new InvalidOperationException($"Medicine with ID {item.MedicineId} not found.");

                        if (!medicine.IsAvailable)
                            throw new InvalidOperationException($"{medicine.Name} is currently unavailable.");

                        if (medicine.StockQuantity < item.Quantity)
                            throw new InvalidOperationException($"Insufficient stock for {medicine.Name}. Available: {medicine.StockQuantity}");

                        if (medicine.RequiresPrescription)
                            needsPrescription = true;

                        orderItems.Add(new OrderItem
                        {
                            MedicineId = item.MedicineId,
                            Quantity = item.Quantity,
                            UnitPrice = medicine.Price
                        });

                        totalAmount += medicine.Price * item.Quantity;
                    }

                    // Validate prescription if required
                    if (needsPrescription)
                    {
                        if (!dto.PrescriptionId.HasValue)
                            throw new InvalidOperationException("A valid prescription is required for one or more items.");

                        var prescription = await _prescriptionRepo.GetByIdAsync(dto.PrescriptionId.Value);
                        if (prescription == null || prescription.UserId != userId)
                            throw new InvalidOperationException("Invalid prescription.");

                        if (prescription.Status != "Approved")
                            throw new InvalidOperationException("Prescription must be approved before ordering.");
                    }

                    var order = new Order
                    {
                        UserId = userId,
                        PrescriptionId = dto.PrescriptionId,
                        TotalAmount = totalAmount,
                        Status = "Pending",
                        ShippingAddress = dto.ShippingAddress ?? user.Address,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        OrderItems = orderItems
                    };

                    // Create payment record
                    var payment = new Payment
                    {
                        Amount = totalAmount,
                        Status = "Pending",
                        Order = order
                    };

                    _context.Orders.Add(order);
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    savedOrder = order;

                    // Send email
                    await _emailHelper.SendOrderConfirmationAsync(
                        user.Email, user.Name, order.Id, totalAmount, order.Status);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return await MapToResponseDto(savedOrder!);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => MapToResponseDtoSync(o));
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : MapToResponseDtoSync(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status, DateTime? fromDate)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status == status);

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return orders.Select(o => MapToResponseDtoSync(o));
        }

        public async Task<OrderResponseDto?> UpdateStatusAsync(int id, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            // If confirming order, deduct inventory
            if (newStatus == "Confirmed" && order.Status == "Pending")
            {
                foreach (var item in order.OrderItems)
                {
                    var medicine = await _medicineRepo.GetByIdAsync(item.MedicineId);
                    if (medicine != null)
                    {
                        medicine.StockQuantity -= item.Quantity;
                        if (medicine.StockQuantity < 0) medicine.StockQuantity = 0;
                        await _medicineRepo.UpdateAsync(medicine);
                    }
                }
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send status update email
            await _emailHelper.SendOrderStatusUpdateAsync(
                order.User.Email, order.User.Name, order.Id, newStatus);

            return MapToResponseDtoSync(order);
        }

        private async Task<OrderResponseDto> MapToResponseDto(Order order)
        {
            var fullOrder = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Medicine)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstAsync(o => o.Id == order.Id);

            return MapToResponseDtoSync(fullOrder);
        }

        private static OrderResponseDto MapToResponseDtoSync(Order order) => new()
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.Name ?? "",
            UserEmail = order.User?.Email ?? "",
            PrescriptionId = order.PrescriptionId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                MedicineId = oi.MedicineId,
                MedicineName = oi.Medicine?.Name ?? "",
                MedicineImageUrl = oi.Medicine?.ImageUrl,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList(),
            Payment = order.Payment == null ? null : new PaymentResponseDto
            {
                Id = order.Payment.Id,
                OrderId = order.Payment.OrderId,
                Amount = order.Payment.Amount,
                Status = order.Payment.Status,
                Method = order.Payment.Method,
                TransactionId = order.Payment.TransactionId,
                PaidAt = order.Payment.PaidAt
            }
        };
    }
}
