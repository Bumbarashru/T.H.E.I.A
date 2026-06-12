using System;
using System.Collections.Generic;
using System.Linq;
using THEIA.Commands;
using THEIA.Core;

namespace THEIA;

class Program
{
    static async Task Main(string[] args)
    {
        var brain = new Brain();

        Console.WriteLine("Проверка всех систем . . .  \n Все системы в норме, готова к работе!");

        while (true)
        {
            Console.Write("> ");
            var request = Console.ReadLine();

            var action = await brain.ProcessCommandAsync(request);

            if (string.IsNullOrEmpty(action)) continue;

            Console.WriteLine(action);
        }
    }
}