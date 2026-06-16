using System;
using System.Diagnostics;
using THEIA.Services.Speech;

namespace THEIA;

public class SystemActions
{
    public static string OpenBrowser()
    {
        Console.WriteLine("DEBAGGING \t ОТКРЫВАЮ БРАУЗЕР . . .");
        return "Сделано!";
    }

    public static string GetTime()
    {
        return $"Локальное время {DateTime.Now:HH:mm}";
    }

    public static string GetDayOfWeek()
    {
        return $"Сегодня: {DateTime.Now:dddd}";
    }
    public static string Sleep(WakeWordDetector _detector)
    {
        _detector.Sleep();
        return "Если понадоблюсь, только скажи!";
    }

    public static string WhoAmI()
    {
        return "Я Тейя, голосовой помошник сделанный Bumbarash, могу сделать много чего! Рада помочь в случае чего <3 \n[Некоторые мои функции находятся в доработке поэтому заранее извиняюсь]";
    }







    
}


