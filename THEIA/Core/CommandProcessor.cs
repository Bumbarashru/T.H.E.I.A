using System;
using THEIA.Commands;
using THEIA.Services.Speech;
using THEIA.Services.Search;

namespace THEIA;

public class CommandProcessor
{
    private readonly WakeWordDetector _detector;
    private readonly SearchGoogle _searchGoogle; 

    public CommandProcessor(WakeWordDetector detector)
    {
        _detector = detector;
        _searchGoogle = new SearchGoogle();
    }
    public async Task<string> Execute (ActionCommand action, string? originalCommand = null)
    {
        switch (action)
        {
            case ActionCommand.open_browser:
            return SystemActions.OpenBrowser();

            case ActionCommand.get_time:
            return SystemActions.GetTime().ToString();

            case ActionCommand.get_week:
            return SystemActions.GetDayOfWeek();

            case ActionCommand.WhoAmI:
            return SystemActions.WhoAmI();

            case ActionCommand.sleep:
            return SystemActions.Sleep(_detector);
            
            case ActionCommand.open_youtube:
            return SystemActions.OpenYoutube();

            case ActionCommand.search:
            return await HandleSearch(originalCommand ?? "");


            default:
            return "Не могу распознать команду";

        }
    }
     private async Task<string> HandleSearch(string command)
    {
        var query = command
        .Replace("найди", "", StringComparison.OrdinalIgnoreCase)
        .Replace("поищи", "", StringComparison.OrdinalIgnoreCase)
        .Replace("загугли", "", StringComparison.OrdinalIgnoreCase)
        .Replace("гугли", "", StringComparison.OrdinalIgnoreCase)
        .Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            return "Не найдено";
        }

        var url = await _searchGoogle.SearchFirstLink(query);

        if (url != null)
        {
            _searchGoogle.OpenInBrowser(url);
            return "Открываю";
        }
        else
        {
            return "Не удалось";
        }

    }
}
