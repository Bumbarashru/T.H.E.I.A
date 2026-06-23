using System;
using Spectre.Console;

namespace THEIA.Ui.UiManager;

public enum EmojiCategory
{
    funny,
    sleep,
    success,
    sad,
    error
    
}
public static class UiManager
{
    private static readonly Dictionary<EmojiCategory, string[]> _emojiCategories = new()
    {
      [EmojiCategory.funny] = ["♪(ᴖ◡ ᴖ)♪", "♡", "(>ᴗ<)", "( •̀ᴗ•́ )و ̑̑", "(° ͜ʖ°)", "ʕ•ᴥ•ʔ", ":D", "^_^", "^.^", "(˶ᵔ ᵕ ᵔ˶)", "₍^. .^₎⟆", "(˶ᵔ ᵕ ᵔ˶)", ""],
      [EmojiCategory.sad] = ["( •_•)", "•∩•", "¬_¬", "*ʖ̯*", ":(", "T_T"],
      [EmojiCategory.success] = ["✓", "ᕦ(òᴥó)ᕥ", "✌(-‿-)✌", "٩( ‘ω’ )و"],
      [EmojiCategory.sleep] = ["Zzz", "(-.-)zzz", "(-.-)"],
      [EmojiCategory.error] = ["X_X", "(╯°□°)╯", "Σ(°△°|||)", "⚠", "✗"]
    };

    private static readonly Random _random = new();

    private static string GetRandomEmoji(EmojiCategory emojiCategory)
    {
        if (_emojiCategories.TryGetValue(emojiCategory, out var emojis))
        {
            return emojis[_random.Next(emojis.Length)];
        }

        return "(>ᴗ<)";
    }
    public static void Print(
        string text,
        string color = "white",
        EmojiCategory? ChooseEmoji = null,
        string? formatting = null,
        string prefix = ">"
        
    )
    {
        string emj = ChooseEmoji.HasValue 
        ? GetRandomEmoji(ChooseEmoji.Value) 
        : "";

        string style = string.IsNullOrEmpty(formatting)? color : $"{formatting} {color}";

        AnsiConsole.MarkupLineInterpolated($"[{style}]{emj} {prefix} {text}[/]");
    }


    //Пока не трожь
    public static void Status(Action action)
    {
        string emj = GetRandomEmoji(EmojiCategory.funny);
        AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("yellow"))
        .Start($"[yellow]{emj} Думаю . . . [/]", ctx =>
        {
            action();
        });
    }
}
