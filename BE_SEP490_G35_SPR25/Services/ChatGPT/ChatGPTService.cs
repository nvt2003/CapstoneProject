using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Planify_BackEnd.Services.ChatGPT
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiKey;
        private readonly string Endpoint;

        public ChatGPTService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            string chatGptPath = configuration["ChatGPT:ChatGptPath"];
            if (string.IsNullOrEmpty(chatGptPath) || !File.Exists(chatGptPath))
            {
                throw new FileNotFoundException("Không tìm thấy file chatgpt.json");
            }

            string json = File.ReadAllText(chatGptPath);
            var config = JsonSerializer.Deserialize<ChatGPTConfig>(json) ?? throw new Exception("Lỗi khi đọc cấu hình ChatGPT");

            ApiKey = config.APIKey;
            Endpoint = config.Endpoint;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        public async Task<string> GetSuggestion(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 4000
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API call failed: {response.StatusCode}, Response: {responseString}");
            }

            var jsonDoc = JsonDocument.Parse(responseString);
            return jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }

    public class ChatGPTConfig
    {
        public string APIKey { get; set; }
        public string Endpoint { get; set; }
    }
}
