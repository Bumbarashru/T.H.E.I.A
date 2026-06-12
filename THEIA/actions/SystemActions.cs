using System;

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

    




    
}


