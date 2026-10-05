using System.Text;
using System.Text.Json;

namespace SobMedidaApi.Services
{
    public class ResumeGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ResumeGeneratorService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GenerateAsync(string prompt, int maxTokens = 4096)
        {
            var apiKey = _configuration["GeminiSettings:ApiKey"]!;
            var model = _configuration["GeminiSettings:Model"]!;
            var apiUrl = _configuration["GeminiSettings:ApiUrl"]!;

            var url = $"{apiUrl}/{model}:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = maxTokens
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {responseBody}");

            using var doc = JsonDocument.Parse(responseBody);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? throw new Exception("Gemini returned an empty response.");
        }
    }
}