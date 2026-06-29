using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using THEIA.Ui.UiManager;

namespace THEIA.Services.Search;

public class SearchGoogle
{
    private readonly HttpClient _httpClient;
    
    public SearchGoogle()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36");
    }

    public async Task<string?> SearchFirstLink(string query)
    {
        try
        {
            UiManager.Print($"🔍 Поиск: {query}");

            // 🔥 Lite версия DuckDuckGo — простой HTML, без капчи!
            var url = $"https://lite.duckduckgo.com/lite/?q={Uri.EscapeDataString(query)}";
            var html = await _httpClient.GetStringAsync(url);

            // 🔥 ДЕБАГ: сохраняем HTML
            await File.WriteAllTextAsync("/tmp/ddg_lite.html", html);

            // 🔥 Простой паттерн для lite.duckduckgo.com
            // Ищем ссылки в таблице результатов
            var matches = Regex.Matches(html, 
                @"<a[^>]+rel=""nofollow""[^>]+href=""([^""]+)""[^>]*>(.*?)</a>",
                RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var foundUrl = match.Groups[1].Value;
                var title = match.Groups[2].Value;
                
                // Фильтруем служебные ссылки DuckDuckGo
                if (!foundUrl.Contains("duckduckgo.com") && 
                    !foundUrl.Contains("duck.com") &&
                    foundUrl.StartsWith("http"))
                {
                    UiManager.Print($"✅ Нашёл: {title}");
                    return foundUrl;
                }
            }

            // Альтернативный паттерн
            var altMatch = Regex.Match(html,
                @"<a[^>]+href=""(https?://(?!duckduckgo|duck\.com)[^""]+)""",
                RegexOptions.Singleline);
            
            if (altMatch.Success)
            {
                UiManager.Print($"✅ Нашёл (alt): {altMatch.Groups[1].Value}");
                return altMatch.Groups[1].Value;
            }

            UiManager.Print("❌ Ссылка не найдена", "yellow");
            return $"https://duckduckgo.com/?q={Uri.EscapeDataString(query)}";
        }
        catch (Exception ex)
        {
            UiManager.Print($"ERROR: {ex.Message}", "red");
            return $"https://duckduckgo.com/?q={Uri.EscapeDataString(query)}";
        }
    }

    public void OpenInBrowser(string url)
    {
        try
        {
            UiManager.Print($"🌐 Открываю: {url}");
            
            if (OperatingSystem.IsLinux())
                Process.Start("xdg-open", url);
            else if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            UiManager.Print($"[error] {ex.Message}", "red");
        }
    }
}