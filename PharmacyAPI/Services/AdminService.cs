using Microsoft.EntityFrameworkCore;
using PharmacyAPI.Data;
using PharmacyAPI.DTOs;

namespace PharmacyAPI.Services
{
    public interface IAdminService
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<InventoryAlertDto>> GetInventoryAlertsAsync(int threshold = 10);
    }

    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;

            return new DashboardDto
            {
                TotalOrdersToday = await _context.Orders
                    .CountAsync(o => o.CreatedAt.Date == today),
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == "Success")
                    .SumAsync(p => p.Amount),
                PendingPrescriptions = await _context.Prescriptions
                    .CountAsync(p => p.Status == "Pending"),
                LowStockMedicines = await _context.Medicines
                    .CountAsync(m => m.StockQuantity < 10 && m.IsAvailable),
                ActiveCustomers = await _context.Users
                    .CountAsync(u => u.Role == "Customer")
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<InventoryAlertDto>> GetInventoryAlertsAsync(int threshold = 10)
        {
            return await _context.Medicines
                .Where(m => m.StockQuantity < threshold && m.IsAvailable)
                .OrderBy(m => m.StockQuantity)
                .Select(m => new InventoryAlertDto
                {
                    MedicineId = m.Id,
                    MedicineName = m.Name,
                    Category = m.Category,
                    StockQuantity = m.StockQuantity
                }).ToListAsync();
        }
    }
}
