using System;
using THEIA.Commands;
using THEIA.Services.Speech;

namespace THEIA;

public class CommandProcessor
{
    private readonly WakeWordDetector _detector;

    public CommandProcessor(WakeWordDetector detector)
    {
        _detector = detector;
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




            default:
            return "Не могу распознать команду";

        }
    }
}
