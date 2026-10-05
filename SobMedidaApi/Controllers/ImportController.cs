using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SobMedidaApi.DTOs;
using SobMedidaApi.Services;

namespace SobMedidaApi.Controllers
{
    [ApiController]
    [Route("import")]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly ImportService _importService;

        public ImportController(ImportService importService)
        {
            _importService = importService;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("preview")]
        [EnableRateLimiting("ai")]
        public async Task<IActionResult> Preview(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only PDF files are accepted." });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size must be under 5MB." });

            using var stream = file.OpenReadStream();
            var summary = await _importService.PreviewImport(stream, GetUserId());
            return Ok(summary);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ImportedProfileDTO data)
        {
            await _importService.ConfirmImport(data, GetUserId());
            return Ok(new { message = "Profile imported successfully." });
        }
    }
}