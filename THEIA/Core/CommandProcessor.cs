using System;
using THEIA.Commands;

namespace THEIA;

public class CommandProcessor
{
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

            //case "reques_ai":
            //return await AskAI(originalCommand ?? "Расскажи что-нибудь интересное");

            default:
            return "Не могу распознать команду";

        }
    }
}
