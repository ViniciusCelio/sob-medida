using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SobMedidaApi.Data;
using SobMedidaApi.DTOs;
using SobMedidaApi.Models;

namespace SobMedidaApi.Controllers
{
    [ApiController]
    [Route("profile/certifications")]
    [Authorize]
    public class CertificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificationController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var certifications = await _context.Certifications
                .Where(c => c.UserId == GetUserId())
                .OrderByDescending(c => c.IssueDate)
                .Select(c => new CertificationDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    IssuingOrganization = c.IssuingOrganization,
                    IssueDate = c.IssueDate,
                    ExpirationDate = c.ExpirationDate,
                    DoesNotExpire = c.DoesNotExpire,
                    CredentialUrl = c.CredentialUrl
                })
                .ToListAsync();

            return Ok(certifications);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CertificationDTO dto)
        {
            var certification = new Certification
            {
                UserId = GetUserId(),
                Name = dto.Name,
                IssuingOrganization = dto.IssuingOrganization,
                IssueDate = dto.IssueDate,
                ExpirationDate = dto.DoesNotExpire ? null : dto.ExpirationDate,
                DoesNotExpire = dto.DoesNotExpire,
                CredentialUrl = dto.CredentialUrl
            };

            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Certification added successfully.", id = certification.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CertificationDTO dto)
        {
            var certification = await _context.Certifications
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());

            if (certification == null)
                return NotFound(new { message = "Certification not found." });

            certification.Name = dto.Name;
            certification.IssuingOrganization = dto.IssuingOrganization;
            certification.IssueDate = dto.IssueDate;
            certification.ExpirationDate = dto.DoesNotExpire ? null : dto.ExpirationDate;
            certification.DoesNotExpire = dto.DoesNotExpire;
            certification.CredentialUrl = dto.CredentialUrl;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Certification updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var certification = await _context.Certifications
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());

            if (certification == null)
                return NotFound(new { message = "Certification not found." });

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Certification deleted successfully." });
        }
    }
}