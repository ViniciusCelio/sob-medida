using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SobMedidaApi.DTOs;
using SobMedidaApi.Models;
using SobMedidaApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SobMedidaApi.Data;

namespace SobMedidaApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            // Check if email is already in use
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "O E-mail já está em uso." });

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { errors });
            }

            var personalInfo = new PersonalInfo
            {
                UserId = user.Id, 
                FullName = dto.FullName,
                Email = dto.Email,
                // Instancia um Address vazio para evitar NullReferenceException 
                // caso o EF Core tente mapear propriedades de navegação obrigatórias.
                Address = new Address() 
            };

            _context.PersonalInfos.Add(personalInfo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "E-mail ou senha inválidos" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "E-mail ou senha inválidos" });

            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName
            });
        }
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new { email, fullName });
        }
    }
}