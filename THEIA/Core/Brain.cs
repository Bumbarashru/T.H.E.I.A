using System;
using THEIA.Commands;
using THEIA.Core;

namespace THEIA;

public class Brain
{
    private readonly IntentMatcher _matcher;
    private readonly CommandProcessor _processor;
    
    public Brain()
    {
        _matcher = new IntentMatcher();
        _processor = new CommandProcessor();
    }

    public async Task<string?> ProcessCommandAsync(string originalCommand)
    {
        if (string.IsNullOrWhiteSpace(originalCommand))
        {
            return "Я тебя не расслышала, повтори пожалуйтса";
        }

        var action = _matcher.Match(originalCommand);

        if (action != null)
        {
            return await _processor.Execute(action.Value, originalCommand);
        }
        else return " e0_0з";
    }
        public ActionCommand? RecognizeCommand(string originalCommand)
    {
        return _matcher.Match(originalCommand);
    }
}
