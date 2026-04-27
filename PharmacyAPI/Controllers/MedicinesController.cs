using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyAPI.DTOs;
using PharmacyAPI.Services;

namespace PharmacyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        /// <summary>
        /// Get all available medicines with optional search and category filter
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? category)
        {
            var medicines = await _medicineService.GetAllAsync(search, category);
            return Ok(medicines);
        }

        /// <summary>
        /// Get a single medicine by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicine = await _medicineService.GetByIdAsync(id);
            if (medicine == null)
                return NotFound(new { message = "Medicine not found." });

            return Ok(medicine);
        }

        /// <summary>
        /// Add a new medicine (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
        {
            var medicine = await _medicineService.CreateAsync(dto);
            return StatusCode(201, medicine);
        }

        /// <summary>
        /// Update a medicine (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineDto dto)
        {
            var medicine = await _medicineService.UpdateAsync(id, dto);
            if (medicine == null)
                return NotFound(new { message = "Medicine not found." });

            return Ok(medicine);
        }

        /// <summary>
        /// Soft delete a medicine (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _medicineService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Medicine not found." });

            return NoContent();
        }
    }
}
