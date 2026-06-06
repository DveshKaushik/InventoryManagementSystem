using System.Text;
using System.Text.Json;

namespace InventoryManagementSystem.Models
{
    public class GroqService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GroqService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            try
            {
                var url = "https://api.groq.com/openai/v1/chat/completions";
                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 500,
                    temperature = 0.3
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseString);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("error", out var error))
                    return $"Error: {error.GetProperty("message").GetString()}";

                return root
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No response received.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}