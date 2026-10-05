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
    [Route("profile/projects")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _context.Projects
                .Where(p => p.UserId == GetUserId())
                .OrderByDescending(p => p.StartDate)
                .Select(p => new ProjectDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Technologies = p.Technologies,
                    ProjectUrl = p.ProjectUrl,
                    GitHubUrl = p.GitHubUrl,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectDTO dto)
        {
            var project = new Project
            {
                UserId = GetUserId(),
                Name = dto.Name,
                Description = dto.Description,
                Technologies = dto.Technologies,
                ProjectUrl = dto.ProjectUrl,
                GitHubUrl = dto.GitHubUrl,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project added successfully.", id = project.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectDTO dto)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());

            if (project == null)
                return NotFound(new { message = "Project not found." });

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.Technologies = dto.Technologies;
            project.ProjectUrl = dto.ProjectUrl;
            project.GitHubUrl = dto.GitHubUrl;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Project updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());

            if (project == null)
                return NotFound(new { message = "Project not found." });

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully." });
        }
    }
}