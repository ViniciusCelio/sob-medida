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
    [Route("profile/experiences")]
    [Authorize]
    public class ExperienceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExperienceController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var experiences = await _context.Experiences
                .Where(e => e.UserId == GetUserId())
                .OrderByDescending(e => e.StartDate)
                .Select(e => new ExperienceDTO
                {
                    Id = e.Id,
                    JobTitle = e.JobTitle,
                    Company = e.Company,
                    Location = e.Location,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrentJob = e.IsCurrentJob,
                    Description = e.Description
                })
                .ToListAsync();

            return Ok(experiences);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExperienceDTO dto)
        {
            var experience = new Experience
            {
                UserId = GetUserId(),
                JobTitle = dto.JobTitle,
                Company = dto.Company,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.IsCurrentJob ? null : dto.EndDate,
                IsCurrentJob = dto.IsCurrentJob,
                Description = dto.Description
            };

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Experience added successfully.", id = experience.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExperienceDTO dto)
        {
            var experience = await _context.Experiences
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == GetUserId());

            if (experience == null)
                return NotFound(new { message = "Experience not found." });

            experience.JobTitle = dto.JobTitle;
            experience.Company = dto.Company;
            experience.Location = dto.Location;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.IsCurrentJob ? null : dto.EndDate;
            experience.IsCurrentJob = dto.IsCurrentJob;
            experience.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Experience updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var experience = await _context.Experiences
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == GetUserId());

            if (experience == null)
                return NotFound(new { message = "Experience not found." });

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Experience deleted successfully." });
        }
    }
}