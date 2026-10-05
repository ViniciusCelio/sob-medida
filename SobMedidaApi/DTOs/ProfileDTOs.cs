using System.ComponentModel.DataAnnotations;

namespace SobMedidaApi.DTOs
{
    public class PersonalInfoDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public AddressDTO Address { get; set; } = new AddressDTO();
        public string LinkedInUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }

    public class AddressDTO
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    public class ExperienceDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJob { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class EducationDTO
    {
        public int Id { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentlyStudying { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class SkillDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }

    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
        public string ProjectUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CertificationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool DoesNotExpire { get; set; }
        public string CredentialUrl { get; set; } = string.Empty;
    }

    public class GenerateResumeRequestDTO
    {
        [MaxLength(200, ErrorMessage = "O título da vaga deve ter no máximo 200 caracteres.")]
        public string JobTitle { get; set; } = string.Empty;

        [MaxLength(15000, ErrorMessage = "A descrição da vaga deve ter no máximo 15.000 caracteres.")]
        public string JobDescription { get; set; } = string.Empty;
    }

    public class GeneratedResumeDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string GeneratedContent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    // --- Import DTOs ---

    public class ImportAddressDTO
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
    }

    public class ImportSkillDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }
    public class ImportSummaryDTO
    {
        public ImportedProfileDTO ExtractedData { get; set; } = new();
        public int NewExperiences { get; set; }
        public int UpdatedExperiences { get; set; }
        public int NewEducations { get; set; }
        public int NewSkills { get; set; }
        public int NewProjects { get; set; }
        public int NewCertifications { get; set; }
        public bool PersonalInfoUpdated { get; set; }
    }

    public class ImportCertificationDTO
    {
        public string Name { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string? IssueDate { get; set; }
        public string? ExpirationDate { get; set; }
        public bool DoesNotExpire { get; set; }
        public string CredentialUrl { get; set; } = string.Empty;
    }

    public class ImportExperienceDTO
    {
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool IsCurrentJob { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ImportEducationDTO
    {
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool IsCurrentlyStudying { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ImportProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
        public string ProjectUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }

    public class ImportedProfileDTO
    {
        public PersonalInfoDTO? PersonalInfo { get; set; }
        public List<ImportExperienceDTO> Experiences { get; set; } = new();
        public List<ImportEducationDTO> Educations { get; set; } = new();
        public List<SkillDTO> Skills { get; set; } = new();
        public List<ImportProjectDTO> Projects { get; set; } = new();
        public List<ImportCertificationDTO> Certifications { get; set; } = new();
    }
}