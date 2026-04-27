using System.ComponentModel.DataAnnotations;

namespace PharmacyAPI.DTOs
{
    // ===== AUTH DTOs =====
    public class RegisterDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // ===== MEDICINE DTOs =====
    public class MedicineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool RequiresPrescription { get; set; }
        public bool IsAvailable { get; set; }
        public string? Dosage { get; set; }
        public string? Packaging { get; set; }
        public string? Manufacturer { get; set; }
    }

    public class CreateMedicineDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool RequiresPrescription { get; set; }

        [MaxLength(100)]
        public string? Dosage { get; set; }

        [MaxLength(100)]
        public string? Packaging { get; set; }

        [MaxLength(200)]
        public string? Manufacturer { get; set; }
    }

    public class UpdateMedicineDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public decimal? Price { get; set; }

        public int? StockQuantity { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool? RequiresPrescription { get; set; }

        public bool? IsAvailable { get; set; }

        [MaxLength(100)]
        public string? Dosage { get; set; }

        [MaxLength(100)]
        public string? Packaging { get; set; }

        [MaxLength(200)]
        public string? Manufacturer { get; set; }
    }

    // ===== PRESCRIPTION DTOs =====
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    public class ReviewPrescriptionDto
    {
        [Required]
        public string Status { get; set; } = string.Empty; // Approved or Rejected

        public string? Reason { get; set; }
    }

    // ===== ORDER DTOs =====
    public class CreateOrderDto
    {
        [Required]
        public List<OrderItemDto> Items { get; set; } = new();

        public int? PrescriptionId { get; set; }

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }
    }

    public class OrderItemDto
    {
        [Required]
        public int MedicineId { get; set; }

        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int? PrescriptionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public PaymentResponseDto? Payment { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? MedicineImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    // ===== PAYMENT DTOs =====
    public class ProcessPaymentDto
    {
        [Required, MaxLength(50)]
        public string Method { get; set; } = string.Empty; // Card, UPI, NetBanking, COD
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Method { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    // ===== ADMIN DTOs =====
    public class DashboardDto
    {
        public int TotalOrdersToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingPrescriptions { get; set; }
        public int LowStockMedicines { get; set; }
        public int ActiveCustomers { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InventoryAlertDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
