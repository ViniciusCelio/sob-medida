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
    [Route("profile/personal-info")]
    [Authorize]
    public class PersonalInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonalInfoController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var info = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.UserId == GetUserId());

            if (info == null)
                return NotFound(new { message = "Personal info not found." });

            return Ok(new PersonalInfoDTO
            {
                Id = info.Id,
                FullName = info.FullName,
                Email = info.Email,
                Phone = info.Phone,
                LinkedInUrl = info.LinkedInUrl,
                GitHubUrl = info.GitHubUrl,
                Summary = info.Summary,
                // Map the Address data
                Address = new AddressDTO
                {
                    Street = info.Address.Street,
                    Number = info.Address.Number,
                    Neighborhood = info.Address.Neighborhood,
                    City = info.Address.City,
                    State = info.Address.State,
                    ZipCode = info.Address.ZipCode
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonalInfoDTO dto)
        {
            var existing = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.UserId == GetUserId());

            if (existing != null)
                return BadRequest(new { message = "Personal info already exists. Use PUT to update." });

            var info = new PersonalInfo
            {
                UserId = GetUserId(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                LinkedInUrl = dto.LinkedInUrl,
                GitHubUrl = dto.GitHubUrl,
                Summary = dto.Summary,
                // Map the Address data
                Address = new Address
                {
                    Street = dto.Address.Street,
                    Number = dto.Address.Number,
                    Neighborhood = dto.Address.Neighborhood,
                    City = dto.Address.City,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                }
            };

            _context.PersonalInfos.Add(info);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Personal info created successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PersonalInfoDTO dto)
        {
            var info = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.UserId == GetUserId());

            if (info == null)
                return NotFound(new { message = "Personal info not found. Use POST to create." });

            info.FullName = dto.FullName;
            info.Email = dto.Email;
            info.Phone = dto.Phone;
            info.LinkedInUrl = dto.LinkedInUrl;
            info.GitHubUrl = dto.GitHubUrl;
            info.Summary = dto.Summary;
            
            // Map the Address data
            info.Address.Street = dto.Address.Street;
            info.Address.Number = dto.Address.Number;
            info.Address.Neighborhood = dto.Address.Neighborhood;
            info.Address.City = dto.Address.City;
            info.Address.State = dto.Address.State;
            info.Address.ZipCode = dto.Address.ZipCode;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Personal info updated successfully." });
        }
    }
}