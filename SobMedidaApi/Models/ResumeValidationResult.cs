namespace SobMedidaApi.Models
{
    public class ResumeValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Violations { get; set; } = new();
    }
}