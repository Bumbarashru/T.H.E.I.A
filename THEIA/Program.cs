using System;
using System.Collections.Generic;
using System.Linq;

namespace THEIA.Core;

class Program
{
    static async Task Main(string[] args)
    {
        var matcher = new IntentMatcher();

        Console.WriteLine("Проверка всех систем . . .  \n Все системы в норме, готова к работе!");

        while (true)
        {
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;
            if (input == "exit") break;

            var command = matcher.Match(input);

            if (command != null)
            {
                Console.WriteLine("ЛОКАЛЬНАЯ КОМАНДА, ВЫПОЛНЯЮ . . .");
            }
            else
            {
                Console.WriteLine("Дайка подумаю . . . <Обращение к Ии>");
            }
        }
    }
}