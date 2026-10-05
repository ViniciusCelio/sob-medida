namespace SobMedidaApi.Models
{
    public class ExtractedJobInfo
    {
        public string JobTitle { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = new();
        public List<string> DesiredSkills { get; set; } = new();
        public List<string> Responsibilities { get; set; } = new();
        public List<string> AtsKeywords { get; set; } = new();
        public string ExperienceLevel { get; set; } = string.Empty;
        public string EducationRequired { get; set; } = string.Empty;
    }
}