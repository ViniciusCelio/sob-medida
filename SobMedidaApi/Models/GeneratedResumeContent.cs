namespace SobMedidaApi.Models
{
    public class GeneratedResumeContent
    {
        public ResumePersonalSection PersonalInfo { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
        public List<ResumeExperienceSection> Experience { get; set; } = new();
        public List<ResumeEducationSection> Education { get; set; } = new();
        public List<ResumeSkillSection> Skills { get; set; } = new();
        public List<ResumeProjectSection> Projects { get; set; } = new();
        public List<ResumeCertificationSection> Certifications { get; set; } = new();
    }

    public class ResumePersonalSection
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string LinkedInUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
    }

    public class ResumeExperienceSection
    {
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public List<string> Bullets { get; set; } = new();
    }

    public class ResumeEducationSection
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
    }

    public class ResumeSkillSection
    {
        public string Category { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
    }

    public class ResumeProjectSection
    {
        public string Name { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
        public List<string> Bullets { get; set; } = new();
    }

    public class ResumeCertificationSection
    {
        public string Name { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}