using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SobMedidaApi.Data;
using SobMedidaApi.DTOs;
using SobMedidaApi.Models;

namespace SobMedidaApi.Services
{
    public class ImportService
    {
        private readonly AppDbContext _context;
        private readonly PdfExtractionService _pdfExtractor;
        private readonly ImportPromptBuilderService _promptBuilder;
        private readonly ResumeGeneratorService _generator;

        public ImportService(
            AppDbContext context,
            PdfExtractionService pdfExtractor,
            ImportPromptBuilderService promptBuilder,
            ResumeGeneratorService generator)
        {
            _context = context;
            _pdfExtractor = pdfExtractor;
            _promptBuilder = promptBuilder;
            _generator = generator;
        }

        public async Task<ImportSummaryDTO> PreviewImport(Stream pdfStream, string userId)
        {
            var rawText = _pdfExtractor.ExtractText(pdfStream);

            if (rawText.Length > 8000)
                rawText = rawText.Substring(0, 8000);

            var prompt = _promptBuilder.BuildExtractionPrompt(rawText);
            var jsonResponse = await _generator.GenerateAsync(prompt, maxTokens: 8192);
            var cleanJson = CleanGeminiResponse(jsonResponse);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ImportedProfileDTO extracted;

            try
            {
                extracted = JsonSerializer.Deserialize<ImportedProfileDTO>(cleanJson, options)
                    ?? throw new Exception("Gemini returned an empty structure.");
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse Gemini response. Raw: {jsonResponse}. Error: {ex.Message}");
            }

            var existingExperiences = await _context.Experiences
                .Where(e => e.UserId == userId).ToListAsync();
            var existingEducations = await _context.Educations
                .Where(e => e.UserId == userId).ToListAsync();
            var existingSkills = await _context.Skills
                .Where(s => s.UserId == userId).ToListAsync();
            var existingProjects = await _context.Projects
                .Where(p => p.UserId == userId).ToListAsync();
            var existingCertifications = await _context.Certifications
                .Where(c => c.UserId == userId).ToListAsync();
            var existingPersonalInfo = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.UserId == userId);

            int newExperiences = 0, updatedExperiences = 0;
            foreach (var exp in extracted.Experiences)
            {
                var match = existingExperiences.FirstOrDefault(e =>
                    e.Company.Trim().ToLower() == exp.Company.Trim().ToLower());
                if (match == null) newExperiences++;
                else updatedExperiences++;
            }

            int newEducations = extracted.Educations.Count(edu =>
                !existingEducations.Any(e =>
                    e.Institution.Trim().ToLower() == edu.Institution.Trim().ToLower()));

            int newSkills = extracted.Skills.Count(skill =>
                !existingSkills.Any(s =>
                    s.Name.Trim().ToLower() == skill.Name.Trim().ToLower()));

            int newProjects = extracted.Projects.Count(proj =>
                !existingProjects.Any(p =>
                    p.Name.Trim().ToLower() == proj.Name.Trim().ToLower()));

            int newCertifications = extracted.Certifications.Count(cert =>
                !existingCertifications.Any(c =>
                    c.Name.Trim().ToLower() == cert.Name.Trim().ToLower()));

            return new ImportSummaryDTO
            {
                ExtractedData = extracted,
                NewExperiences = newExperiences,
                UpdatedExperiences = updatedExperiences,
                NewEducations = newEducations,
                NewSkills = newSkills,
                NewProjects = newProjects,
                NewCertifications = newCertifications,
                PersonalInfoUpdated = existingPersonalInfo != null
            };
        }

        public async Task ConfirmImport(ImportedProfileDTO data, string userId)
        {
            var existing = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (data.PersonalInfo != null)
            {
                if (existing == null)
                {
                    _context.PersonalInfos.Add(new PersonalInfo
                    {
                        UserId = userId,
                        FullName = data.PersonalInfo.FullName,
                        Email = data.PersonalInfo.Email,
                        Phone = data.PersonalInfo.Phone,
                        Address = new Address
                        {
                            Street = data.PersonalInfo.Address.Street,
                            Number = data.PersonalInfo.Address.Number,
                            Neighborhood = data.PersonalInfo.Address.Neighborhood,
                            City = data.PersonalInfo.Address.City,
                            State = data.PersonalInfo.Address.State,
                            ZipCode = data.PersonalInfo.Address.ZipCode
                        },
                        LinkedInUrl = data.PersonalInfo.LinkedInUrl,
                        GitHubUrl = data.PersonalInfo.GitHubUrl,
                        Summary = data.PersonalInfo.Summary
                    });
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(existing.FullName))
                        existing.FullName = data.PersonalInfo.FullName;
                    if (string.IsNullOrWhiteSpace(existing.Email))
                        existing.Email = data.PersonalInfo.Email;
                    if (string.IsNullOrWhiteSpace(existing.Phone))
                        existing.Phone = data.PersonalInfo.Phone;
                    if (string.IsNullOrWhiteSpace(existing.LinkedInUrl))
                        existing.LinkedInUrl = data.PersonalInfo.LinkedInUrl;
                    if (string.IsNullOrWhiteSpace(existing.GitHubUrl))
                        existing.GitHubUrl = data.PersonalInfo.GitHubUrl;
                    if (string.IsNullOrWhiteSpace(existing.Summary))
                        existing.Summary = data.PersonalInfo.Summary;
                    if (string.IsNullOrWhiteSpace(existing.Address.City))
                        existing.Address.City = data.PersonalInfo.Address.City;
                    if (string.IsNullOrWhiteSpace(existing.Address.State))
                        existing.Address.State = data.PersonalInfo.Address.State;
                    if (string.IsNullOrWhiteSpace(existing.Address.ZipCode))
                        existing.Address.ZipCode = data.PersonalInfo.Address.ZipCode;
                    if (string.IsNullOrWhiteSpace(existing.Address.Street))
                        existing.Address.Street = data.PersonalInfo.Address.Street;
                }
            }

            var existingExperiences = await _context.Experiences
                .Where(e => e.UserId == userId).ToListAsync();

            foreach (var exp in data.Experiences)
            {
                var match = existingExperiences.FirstOrDefault(e =>
                    e.Company.Trim().ToLower() == exp.Company.Trim().ToLower());

                if (match == null)
                {
                    _context.Experiences.Add(new Experience
                    {
                        UserId = userId,
                        JobTitle = exp.JobTitle,
                        Company = exp.Company,
                        Location = exp.Location,
                        StartDate = ParseDate(exp.StartDate),
                        EndDate = exp.IsCurrentJob ? null : ParseNullableDate(exp.EndDate),
                        IsCurrentJob = exp.IsCurrentJob,
                        Description = exp.Description
                    });
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(exp.Description) &&
                        !match.Description.Contains(exp.Description.Trim()))
                    {
                        match.Description = string.IsNullOrWhiteSpace(match.Description)
                            ? exp.Description
                            : $"{match.Description}\n\n{exp.Description}";
                    }
                    if (exp.IsCurrentJob) match.IsCurrentJob = true;
                }
            }

            var existingEducations = await _context.Educations
                .Where(e => e.UserId == userId).ToListAsync();

            foreach (var edu in data.Educations)
            {
                var exists = existingEducations.Any(e =>
                    e.Institution.Trim().ToLower() == edu.Institution.Trim().ToLower());

                if (!exists)
                {
                    _context.Educations.Add(new Education
                    {
                        UserId = userId,
                        Degree = edu.Degree,
                        FieldOfStudy = edu.FieldOfStudy,
                        Institution = edu.Institution,
                        Location = edu.Location,
                        StartDate = ParseDate(edu.StartDate),
                        EndDate = edu.IsCurrentlyStudying ? null : ParseNullableDate(edu.EndDate),
                        IsCurrentlyStudying = edu.IsCurrentlyStudying,
                        Description = edu.Description
                    });
                }
            }

            var existingSkills = await _context.Skills
                .Where(s => s.UserId == userId).ToListAsync();

            foreach (var skill in data.Skills)
            {
                var exists = existingSkills.Any(s =>
                    s.Name.Trim().ToLower() == skill.Name.Trim().ToLower());

                if (!exists)
                {
                    _context.Skills.Add(new Skill
                    {
                        UserId = userId,
                        Name = skill.Name,
                        Category = skill.Category,
                        Level = skill.Level
                    });
                }
            }

            var existingProjects = await _context.Projects
                .Where(p => p.UserId == userId).ToListAsync();

            foreach (var proj in data.Projects)
            {
                var exists = existingProjects.Any(p =>
                    p.Name.Trim().ToLower() == proj.Name.Trim().ToLower());

                if (!exists)
                {
                    _context.Projects.Add(new Project
                    {
                        UserId = userId,
                        Name = proj.Name,
                        Description = proj.Description,
                        Technologies = proj.Technologies,
                        ProjectUrl = proj.ProjectUrl,
                        GitHubUrl = proj.GitHubUrl,
                        StartDate = ParseNullableDate(proj.StartDate),
                        EndDate = ParseNullableDate(proj.EndDate)
                    });
                }
            }

            var existingCerts = await _context.Certifications
                .Where(c => c.UserId == userId).ToListAsync();

            foreach (var cert in data.Certifications)
            {
                var exists = existingCerts.Any(c =>
                    c.Name.Trim().ToLower() == cert.Name.Trim().ToLower());

                if (!exists)
                {
                    _context.Certifications.Add(new Certification
                    {
                        UserId = userId,
                        Name = cert.Name,
                        IssuingOrganization = cert.IssuingOrganization,
                        IssueDate = ParseDate(cert.IssueDate),
                        ExpirationDate = cert.DoesNotExpire
                            ? null
                            : ParseNullableDate(cert.ExpirationDate),
                        DoesNotExpire = cert.DoesNotExpire,
                        CredentialUrl = cert.CredentialUrl
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private DateTime ParseDate(string? dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return DateTime.UtcNow;

            if (DateTime.TryParse(dateStr, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return DateTime.UtcNow;
        }

        private DateTime? ParseNullableDate(string? dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr)) return null;
            if (dateStr.Equals("null", StringComparison.OrdinalIgnoreCase)) return null;

            if (DateTime.TryParse(dateStr, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return null;
        }
        public string ExtractTextPublic(Stream stream)
        {
            return _pdfExtractor.ExtractText(stream);
        }

        private string CleanGeminiResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "{}";

            raw = raw.Trim();

            // Remove markdown code fences
            if (raw.StartsWith("```json"))
                raw = raw.Substring(7);
            else if (raw.StartsWith("```"))
                raw = raw.Substring(3);

            if (raw.EndsWith("```"))
                raw = raw.Substring(0, raw.Length - 3);

            raw = raw.Trim();

            // If JSON is truncated, attempt to close it gracefully
            if (!raw.EndsWith("}"))
            {
                // Find the last complete, valid array or object entry
                // by trimming to the last complete closing brace/bracket
                var lastValidClose = Math.Max(
                    raw.LastIndexOf("},"),
                    raw.LastIndexOf("}]")
                );

                if (lastValidClose > 0)
                {
                    // Cut after the last valid closing and close all open structures
                    raw = raw.Substring(0, lastValidClose + 1);

                    // Count unclosed brackets and braces and close them
                    var openBraces = raw.Count(c => c == '{') - raw.Count(c => c == '}');
                    var openBrackets = raw.Count(c => c == '[') - raw.Count(c => c == ']');

                    for (int i = 0; i < openBrackets; i++) raw += "]";
                    for (int i = 0; i < openBraces; i++) raw += "}";
                }
                else
                {
                    return "{}";
                }
            }

            return raw;
        }
    }
}