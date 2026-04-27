using PharmacyAPI.DTOs;
using PharmacyAPI.Helpers;
using PharmacyAPI.Models;
using PharmacyAPI.Repositories;

namespace PharmacyAPI.Services
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> UploadAsync(int userId, IFormFile file);
        Task<IEnumerable<PrescriptionDto>> GetByUserAsync(int userId);
        Task<PrescriptionDto?> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionDto>> GetAllAsync(string? status);
        Task<PrescriptionDto?> ReviewAsync(int id, ReviewPrescriptionDto dto);
    }

    public class PrescriptionService : IPrescriptionService
    {
        private readonly IRepository<Prescription> _prescriptionRepo;
        private readonly IRepository<User> _userRepo;
        private readonly FileHelper _fileHelper;
        private readonly IEmailHelper _emailHelper;

        public PrescriptionService(
            IRepository<Prescription> prescriptionRepo,
            IRepository<User> userRepo,
            FileHelper fileHelper,
            IEmailHelper emailHelper)
        {
            _prescriptionRepo = prescriptionRepo;
            _userRepo = userRepo;
            _fileHelper = fileHelper;
            _emailHelper = emailHelper;
        }

        public async Task<PrescriptionDto> UploadAsync(int userId, IFormFile file)
        {
            var imageUrl = await _fileHelper.SavePrescriptionAsync(file);

            var prescription = new Prescription
            {
                UserId = userId,
                ImageUrl = imageUrl,
                Status = "Pending",
                UploadedAt = DateTime.UtcNow
            };

            await _prescriptionRepo.AddAsync(prescription);

            var user = await _userRepo.GetByIdAsync(userId);

            return new PrescriptionDto
            {
                Id = prescription.Id,
                UserId = prescription.UserId,
                UserName = user?.Name ?? "",
                ImageUrl = prescription.ImageUrl,
                Status = prescription.Status,
                UploadedAt = prescription.UploadedAt
            };
        }

        public async Task<IEnumerable<PrescriptionDto>> GetByUserAsync(int userId)
        {
            var prescriptions = await _prescriptionRepo.FindAsync(p => p.UserId == userId);
            var user = await _userRepo.GetByIdAsync(userId);

            return prescriptions.OrderByDescending(p => p.UploadedAt).Select(p => new PrescriptionDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = user?.Name ?? "",
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                Reason = p.Reason,
                UploadedAt = p.UploadedAt,
                ReviewedAt = p.ReviewedAt
            });
        }

        public async Task<PrescriptionDto?> GetByIdAsync(int id)
        {
            var p = await _prescriptionRepo.GetByIdAsync(id);
            if (p == null) return null;

            var user = await _userRepo.GetByIdAsync(p.UserId);

            return new PrescriptionDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = user?.Name ?? "",
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                Reason = p.Reason,
                UploadedAt = p.UploadedAt,
                ReviewedAt = p.ReviewedAt
            };
        }

        public async Task<IEnumerable<PrescriptionDto>> GetAllAsync(string? status)
        {
            IEnumerable<Prescription> prescriptions;

            if (!string.IsNullOrWhiteSpace(status))
                prescriptions = await _prescriptionRepo.FindAsync(p => p.Status == status);
            else
                prescriptions = await _prescriptionRepo.GetAllAsync();

            var result = new List<PrescriptionDto>();
            foreach (var p in prescriptions.OrderByDescending(p => p.UploadedAt))
            {
                var user = await _userRepo.GetByIdAsync(p.UserId);
                result.Add(new PrescriptionDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = user?.Name ?? "",
                    ImageUrl = p.ImageUrl,
                    Status = p.Status,
                    Reason = p.Reason,
                    UploadedAt = p.UploadedAt,
                    ReviewedAt = p.ReviewedAt
                });
            }

            return result;
        }

        public async Task<PrescriptionDto?> ReviewAsync(int id, ReviewPrescriptionDto dto)
        {
            var p = await _prescriptionRepo.GetByIdAsync(id);
            if (p == null) return null;

            p.Status = dto.Status;
            p.Reason = dto.Reason;
            p.ReviewedAt = DateTime.UtcNow;

            await _prescriptionRepo.UpdateAsync(p);

            var user = await _userRepo.GetByIdAsync(p.UserId);

            // Send email notification
            if (user != null)
            {
                await _emailHelper.SendPrescriptionStatusAsync(
                    user.Email, user.Name, p.Id, p.Status, p.Reason);
            }

            return new PrescriptionDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = user?.Name ?? "",
                ImageUrl = p.ImageUrl,
                Status = p.Status,
                Reason = p.Reason,
                UploadedAt = p.UploadedAt,
                ReviewedAt = p.ReviewedAt
            };
        }
    }
}
