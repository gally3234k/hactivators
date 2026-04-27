using PharmacyAPI.DTOs;
using PharmacyAPI.Models;
using PharmacyAPI.Repositories;

namespace PharmacyAPI.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<MedicineDto>> GetAllAsync(string? search, string? category);
        Task<MedicineDto?> GetByIdAsync(int id);
        Task<MedicineDto> CreateAsync(CreateMedicineDto dto);
        Task<MedicineDto?> UpdateAsync(int id, UpdateMedicineDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class MedicineService : IMedicineService
    {
        private readonly IRepository<Medicine> _medicineRepo;

        public MedicineService(IRepository<Medicine> medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        public async Task<IEnumerable<MedicineDto>> GetAllAsync(string? search, string? category)
        {
            var medicines = await _medicineRepo.GetAllAsync();
            var query = medicines.Where(m => m.IsAvailable);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                          (m.Description != null && m.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(m => m.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            return query.Select(MapToDto);
        }

        public async Task<MedicineDto?> GetByIdAsync(int id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);
            return medicine == null ? null : MapToDto(medicine);
        }

        public async Task<MedicineDto> CreateAsync(CreateMedicineDto dto)
        {
            var medicine = new Medicine
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                RequiresPrescription = dto.RequiresPrescription,
                IsAvailable = true,
                Dosage = dto.Dosage,
                Packaging = dto.Packaging,
                Manufacturer = dto.Manufacturer,
                CreatedAt = DateTime.UtcNow
            };

            await _medicineRepo.AddAsync(medicine);
            return MapToDto(medicine);
        }

        public async Task<MedicineDto?> UpdateAsync(int id, UpdateMedicineDto dto)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);
            if (medicine == null) return null;

            if (dto.Name != null) medicine.Name = dto.Name;
            if (dto.Description != null) medicine.Description = dto.Description;
            if (dto.Category != null) medicine.Category = dto.Category;
            if (dto.Price.HasValue) medicine.Price = dto.Price.Value;
            if (dto.StockQuantity.HasValue) medicine.StockQuantity = dto.StockQuantity.Value;
            if (dto.ImageUrl != null) medicine.ImageUrl = dto.ImageUrl;
            if (dto.RequiresPrescription.HasValue) medicine.RequiresPrescription = dto.RequiresPrescription.Value;
            if (dto.IsAvailable.HasValue) medicine.IsAvailable = dto.IsAvailable.Value;
            if (dto.Dosage != null) medicine.Dosage = dto.Dosage;
            if (dto.Packaging != null) medicine.Packaging = dto.Packaging;
            if (dto.Manufacturer != null) medicine.Manufacturer = dto.Manufacturer;

            await _medicineRepo.UpdateAsync(medicine);
            return MapToDto(medicine);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);
            if (medicine == null) return false;

            medicine.IsAvailable = false;
            await _medicineRepo.UpdateAsync(medicine);
            return true;
        }

        private static MedicineDto MapToDto(Medicine m) => new()
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            Category = m.Category,
            Price = m.Price,
            StockQuantity = m.StockQuantity,
            ImageUrl = m.ImageUrl,
            RequiresPrescription = m.RequiresPrescription,
            IsAvailable = m.IsAvailable,
            Dosage = m.Dosage,
            Packaging = m.Packaging,
            Manufacturer = m.Manufacturer
        };
    }
}
