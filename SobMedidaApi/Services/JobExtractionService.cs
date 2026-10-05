using System.Text.Json;
using SobMedidaApi.Models;

namespace SobMedidaApi.Services
{
    public class JobExtractionService
    {
        private readonly PromptBuilderService _promptBuilder;
        private readonly ResumeGeneratorService _generator;

        public JobExtractionService(
            PromptBuilderService promptBuilder,
            ResumeGeneratorService generator)
        {
            _promptBuilder = promptBuilder;
            _generator = generator;
        }

        public async Task<ExtractedJobInfo> ExtractAsync(string jobDescription)
        {
            var prompt = _promptBuilder.BuildJobExtractionPrompt(jobDescription);
            var rawResponse = await _generator.GenerateAsync(prompt, maxTokens: 4096);
            var cleanJson = CleanResponse(rawResponse);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ExtractedJobInfo>(cleanJson, options)
                ?? throw new Exception("A IA não retornou uma estrutura válida para a descrição da vaga.");

            return result;
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

            raw = raw.Trim();

            if (!raw.EndsWith("}"))
            {
                var lastValidClose = Math.Max(
                    raw.LastIndexOf("},"),
                    raw.LastIndexOf("}]")
                );

                if (lastValidClose > 0)
                {
                    raw = raw.Substring(0, lastValidClose + 1);

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