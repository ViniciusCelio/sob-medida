using System.Text.Json;
using SobMedidaApi.Models;

namespace SobMedidaApi.Services
{
    public class ResumeValidationService
    {
        private readonly PromptBuilderService _promptBuilder;
        private readonly ResumeGeneratorService _generator;

        public ResumeValidationService(
            PromptBuilderService promptBuilder,
            ResumeGeneratorService generator)
        {
            _promptBuilder = promptBuilder;
            _generator = generator;
        }

        public async Task<ResumeValidationResult> ValidateAsync(
            string profileBlock,
            string generatedResumeJson)
        {
            var prompt = _promptBuilder.BuildValidationPrompt(
                profileBlock,
                generatedResumeJson
            );

            var rawResponse = await _generator.GenerateAsync(prompt, maxTokens: 2048);
            var cleanJson = CleanResponse(rawResponse);

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<ResumeValidationResult>(cleanJson, options)
                    ?? new ResumeValidationResult { IsValid = false, Violations = new List<string> { "Não foi possível interpretar o resultado da validação." } };
            }
            catch
            {
                return new ResumeValidationResult
                {
                    IsValid = false,
                    Violations = new List<string> { "Erro interno ao processar a validação do currículo." }
                };
            }
        }

        private string CleanResponse(string raw)
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
    }
}