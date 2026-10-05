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
    [Route("profile/educations")]
    [Authorize]
    public class EducationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EducationController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var educations = await _context.Educations
                .Where(e => e.UserId == GetUserId())
                .OrderByDescending(e => e.StartDate)
                .Select(e => new EducationDTO
                {
                    Id = e.Id,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    Institution = e.Institution,
                    Location = e.Location,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrentlyStudying = e.IsCurrentlyStudying,
                    Description = e.Description
                })
                .ToListAsync();

            return Ok(educations);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EducationDTO dto)
        {
            var education = new Education
            {
                UserId = GetUserId(),
                Degree = dto.Degree,
                FieldOfStudy = dto.FieldOfStudy,
                Institution = dto.Institution,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentlyStudying ? null : dto.EndDate,
                IsCurrentlyStudying = dto.IsCurrentlyStudying,
                Description = dto.Description
            };

            _context.Educations.Add(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Education added successfully.", id = education.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EducationDTO dto)
        {
            var education = await _context.Educations
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == GetUserId());

            if (education == null)
                return NotFound(new { message = "Education not found." });

            education.Degree = dto.Degree;
            education.FieldOfStudy = dto.FieldOfStudy;
            education.Institution = dto.Institution;
            education.Location = dto.Location;
            education.StartDate = dto.StartDate;
            education.EndDate = dto.IsCurrentlyStudying ? null : dto.EndDate;
            education.IsCurrentlyStudying = dto.IsCurrentlyStudying;
            education.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Education updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var education = await _context.Educations
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == GetUserId());

            if (education == null)
                return NotFound(new { message = "Education not found." });

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Education deleted successfully." });
        }
    }
}