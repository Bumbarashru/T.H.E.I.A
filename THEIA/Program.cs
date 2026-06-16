using System;
using System.Collections.Generic;
using System.Linq;
using THEIA.Commands;
using THEIA.Core;
using THEIA.Services.Speech;

namespace THEIA;

class Program
{
    static async Task Main(string[] args)
    {
        var brain = new Brain();

        Console.WriteLine("Проверка всех систем . . .  ");
        Console.WriteLine("All Right");

        var detector = new WakeWordDetector(
            keywordsFile: "Data/Models/kws/keywords.txt",
            kwsModelPath: "Data/Models/kws",
            asrModelPath: "Data/Models/asr-ru"
        );

        detector.WakeWordDetected += () =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Да, да ... ?");
            Console.ResetColor();
        };

        detector.CommandRecognized +=  async (commandText) =>
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"👂 Вы услышали: \"{commandText}\"");
            Console.ResetColor();
            
            // Передаём команду в мозг
            var response = await brain.ProcessCommandAsync(commandText);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"🧠 ТЕИА: {response}");
            Console.ResetColor();
        };
        

        Console.WriteLine("=== ТЕИА запускается ===");
        Console.WriteLine("Скажи 'тея' чтобы разбудить...");
        Console.WriteLine("Нажми Ctrl+C для выхода\n");
        
        detector.Start();
        
        // 6. Ждём нажатия Ctrl+C
        var exitEvent = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            exitEvent.Set();
        };
        
        exitEvent.Wait();
        
        // 7. Корректное завершение
        Console.WriteLine("\nОстанавливаем ТЕИА...");
        detector.Stop();
        detector.Dispose();
        Console.WriteLine("До свидания!");
    }
}