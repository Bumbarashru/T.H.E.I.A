using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using THEIA.Ui.UiManager;

namespace THEIA.Services.AI;

public class AiHandler{
    private readonly HttpClient _httpClient;

    private readonly string _ollamaUrl = "http://localhost:11434";
    private readonly string _model = "qwen2.5:7b";

    private const string SystemPrompt = @"
    Ты — ТЕИА, дружелюбный и умный голосовой помощник.
    Отвечай кратко (1-3 предложения), живо и с юмором.
    Используй эмодзи уместно. Говори от первого лица.
    Никогда не упоминай, что ты ИИ или языковая модель.
    Ты общаешься голосом, поэтому не используй markdown, списки и спецсимволы.
    Отвечай только на Русском
    ";

    public AiHandler()
    {
        _httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(30) };

        UiManager.Print($"Модель {_model}","yellow", formatting:"bold");
        UiManager.Print($"Ссылка {_ollamaUrl}","yellow", formatting:"bold");
    }
    public async Task<string?> AskAsync(string userMessage)
    {
        try
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = userMessage }
                },
                stream = false,
                options = new
                {
                    temperature = 0.7,
                    top_p = 0.9,
                    num_predict = 200
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/chat",content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
            
            return responseObj
                .GetProperty("message")
                .GetProperty("content")
                .GetString()?.Trim();
        }
        catch (HttpRequestException ex)
        {
            UiManager.Print($"[ERROR] Тея_AI не отвечает: {ex.Message}", "red", EmojiCategory.sad, "bold");
            return "Упс, нейронка спит. Проверь Docker.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
            return "Что-то пошло не так. Попробуй ещё раз.";
        }
    }

}