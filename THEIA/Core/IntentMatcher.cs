using System;
using System.Collections.Generic;
using System.Linq;

namespace THEIA.Core;

class IntentMatcher
{
    private readonly Dictionary<string, string> _command = new()
    {
        //Системные команды
        ["открой браузер"] = "open_browser",
        ["загугли"] = "open_browser",
        ["открой яндекс"] = "open_browser",
        ["открой гугл"] = "open_browser",
        ["выключи комп"] = "shutdown",

        //Информационные команды
        ["сколько время"] = "get_time",
        ["время"] = "get_time",
        ["дата"] = "get_date",
        ["кто ты"] = "who_are_you",

        //EXIT
        ["конец связи"] = "exit",
        ["Пока"] = "exit",
        ["выход"] = "exit"
    };

    private readonly Dictionary<string,string> _synonyms = new()
    {
        ["гугл"] = "браузер",
        ["интернет"] = "браузер",
        ["инет"] = "браузер"
    };

    public string? Match(string userInput)
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