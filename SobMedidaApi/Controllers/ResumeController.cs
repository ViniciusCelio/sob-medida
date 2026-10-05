using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SobMedidaApi.Data;
using SobMedidaApi.DTOs;
using SobMedidaApi.Models;
using SobMedidaApi.Services;

namespace SobMedidaApi.Controllers
{
    [ApiController]
    [Route("resume")]
    [Authorize]
    public class ResumeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PromptBuilderService _promptBuilder;
        private readonly ResumeGeneratorService _generator;
        private readonly JobExtractionService _jobExtractor;
        private readonly ResumeValidationService _validator;

        public ResumeController(
            AppDbContext context,
            PromptBuilderService promptBuilder,
            ResumeGeneratorService generator,
            JobExtractionService jobExtractor,
            ResumeValidationService validator)
        {
            _context = context;
            _promptBuilder = promptBuilder;
            _generator = generator;
            _jobExtractor = jobExtractor;
            _validator = validator;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateResumeRequestDTO dto)
        {
            var userId = GetUserId();

            // Etapa 1 — Extrair e estruturar a descrição da vaga
            ExtractedJobInfo jobInfo;
            try
            {
                jobInfo = await _jobExtractor.ExtractAsync(dto.JobDescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Não foi possível processar a descrição da vaga. Verifique se ela contém informações suficientes sobre os requisitos do cargo e tente novamente.",
                    detail = ex.Message
                });
            }

            if (!jobInfo.RequiredSkills.Any() &&
                !jobInfo.Responsibilities.Any() &&
                !jobInfo.AtsKeywords.Any())
            {
                return BadRequest(new
                {
                    message = "Não foi possível identificar requisitos técnicos na descrição da vaga. Certifique-se de incluir as habilidades exigidas e as responsabilidades do cargo."
                });
            }

            // Etapa 2 — Carregar o perfil do candidato
            var profileBlock = await _promptBuilder.BuildProfileBlock(userId);
            var prompt = await _promptBuilder.BuildPrompt(userId, dto.JobTitle, jobInfo);

            // Etapa 3 — Gerar e validar o currículo com até 2 tentativas
            string cleanJson = string.Empty;
            ResumeValidationResult? validation = null;
            int maxAttempts = 2;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var rawResponse = await _generator.GenerateAsync(prompt, maxTokens: 8192);
                cleanJson = CleanGeminiResponse(rawResponse);

                try
                {
                    using var doc = JsonDocument.Parse(cleanJson);
                }
                catch (JsonException ex)
                {
                    if (attempt == maxAttempts)
                        return StatusCode(500, new { message = $"Erro ao processar a resposta da IA: {ex.Message}" });

                    continue;
                }

                // Etapa 4 — Validar o currículo gerado
                validation = await _validator.ValidateAsync(profileBlock, cleanJson);

                if (validation.IsValid)
                    break;

                if (attempt == maxAttempts)
                {
                    return StatusCode(422, new
                    {
                        message = "O currículo gerado contém inconsistências em relação ao seu perfil e não pôde ser corrigido automaticamente. Tente novamente.",
                        violations = validation.Violations
                    });
                }
            }

            // Etapa 5 — Salvar o currículo validado
            var resume = new GeneratedResume
            {
                UserId = userId,
                JobTitle = dto.JobTitle,
                JobDescription = dto.JobDescription,
                GeneratedContent = cleanJson,
                CreatedAt = DateTime.UtcNow
            };

            _context.GeneratedResumes.Add(resume);
            await _context.SaveChangesAsync();

            return Ok(new GeneratedResumeDTO
            {
                Id = resume.Id,
                JobTitle = resume.JobTitle,
                JobDescription = resume.JobDescription,
                GeneratedContent = resume.GeneratedContent,
                CreatedAt = resume.CreatedAt
            });
        }
        private string CleanGeminiResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "{}";

            raw = raw.Trim();

            if (raw.StartsWith("```json"))
                raw = raw.Substring(7);
            else if (raw.StartsWith("```"))
                raw = raw.Substring(3);

            if (raw.EndsWith("```"))
                raw = raw.Substring(0, raw.Length - 3);

            return raw.Trim();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var resumes = await _context.GeneratedResumes
                .Where(r => r.UserId == GetUserId())
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new GeneratedResumeDTO
                {
                    Id = r.Id,
                    JobTitle = r.JobTitle,
                    JobDescription = r.JobDescription,
                    GeneratedContent = r.GeneratedContent,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(resumes);
        }

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resume = await _context.GeneratedResumes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == GetUserId());

            if (resume == null)
                return NotFound(new { message = "Resume not found." });

            return Ok(new GeneratedResumeDTO
            {
                Id = resume.Id,
                JobTitle = resume.JobTitle,
                JobDescription = resume.JobDescription,
                GeneratedContent = resume.GeneratedContent,
                CreatedAt = resume.CreatedAt
            });
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resume = await _context.GeneratedResumes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == GetUserId());

            if (resume == null)
                return NotFound(new { message = "Resume not found." });

            _context.GeneratedResumes.Remove(resume);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Resume deleted from history." });
        }
    }
}