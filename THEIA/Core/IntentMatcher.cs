using System;
using System.Collections.Generic;
using System.Linq;
using THEIA.Commands;


namespace THEIA.Core;


class IntentMatcher
{
    private readonly Dictionary<string, ActionCommand> _command = new()
    {
        //Системные команды
        ["открой браузер"] = ActionCommand.open_browser,
        ["загугли"] = ActionCommand.open_browser,
        ["выключи комп"] = ActionCommand.shutdown,

        //Информационные команды
        ["сколько время"] = ActionCommand.get_time,
        ["время"] = ActionCommand.get_time,
        ["день недели"] = ActionCommand.get_week,
        ["кто ты"] = ActionCommand.WhoAmI,

        //EXIT
        ["конец связи"] = ActionCommand.exit,
        ["пока"] = ActionCommand.exit,
        ["выход"] = ActionCommand.exit
    };

    private readonly Dictionary<string,string> _synonyms = new()
    {
        ["гугл"] = "браузер",
        ["интернет"] = "браузер",
        ["инет"] = "браузер"
    };

    public ActionCommand? Match(string userInput)
    {
        var normalized = NormalizeText(userInput);
        foreach(var command in _command)
        {
            if(normalized.Contains(command.Key)) return command.Value;
        }

        foreach (var synonym in _synonyms)
        {
            if (normalized.Contains(synonym.Key))
            {
                foreach(var command in _command)
                {
                    if(command.Key.Contains(synonym.Value)) return command.Value;
                }
            }
        }


        return null;
    }

    private string NormalizeText(string text)
    {
        return text
            .ToLower()
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Replace("  ", " ")
            .Replace(",", "")
            .Trim();
    }
}