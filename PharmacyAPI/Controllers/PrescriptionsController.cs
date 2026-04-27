using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyAPI.DTOs;
using PharmacyAPI.Services;

namespace PharmacyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        private int GetUserId() => int.Parse(User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        /// <summary>
        /// Upload a prescription image (Customer)
        /// </summary>
        [HttpPost("upload")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid file." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Only JPG, PNG, and PDF files are allowed." });

            var result = await _prescriptionService.UploadAsync(GetUserId(), file);
            return StatusCode(201, result);
        }

        /// <summary>
        /// Get all prescriptions uploaded by the logged-in user
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMy()
        {
            var prescriptions = await _prescriptionService.GetByUserAsync(GetUserId());
            return Ok(prescriptions);
        }

        /// <summary>
        /// Get a single prescription by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound(new { message = "Prescription not found." });

            // Customers can only see their own
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Customer" && prescription.UserId != GetUserId())
                return Forbid();

            return Ok(prescription);
        }

        /// <summary>
        /// Get all prescriptions (Admin/Pharmacist), optionally filter by status
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var prescriptions = await _prescriptionService.GetAllAsync(status);
            return Ok(prescriptions);
        }

        /// <summary>
        /// Approve or reject a prescription (Admin/Pharmacist)
        /// </summary>
        [HttpPatch("{id}/review")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Review(int id, [FromBody] ReviewPrescriptionDto dto)
        {
            if (dto.Status != "Approved" && dto.Status != "Rejected")
                return BadRequest(new { message = "Status must be 'Approved' or 'Rejected'." });

            var result = await _prescriptionService.ReviewAsync(id, dto);
            if (result == null)
                return NotFound(new { message = "Prescription not found." });

            return Ok(result);
        }
    }
}
