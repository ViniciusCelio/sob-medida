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
    [Route("profile/skills")]
    [Authorize]
    public class SkillController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var skills = await _context.Skills
                .Where(s => s.UserId == GetUserId())
                .OrderBy(s => s.Category)
                .Select(s => new SkillDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    Level = s.Level
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillDTO dto)
        {
            var skill = new Skill
            {
                UserId = GetUserId(),
                Name = dto.Name,
                Category = dto.Category,
                Level = dto.Level
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill added successfully.", id = skill.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SkillDTO dto)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == GetUserId());

            if (skill == null)
                return NotFound(new { message = "Skill not found." });

            skill.Name = dto.Name;
            skill.Category = dto.Category;
            skill.Level = dto.Level;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == GetUserId());

            if (skill == null)
                return NotFound(new { message = "Skill not found." });

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Skill deleted successfully." });
        }
    }
}