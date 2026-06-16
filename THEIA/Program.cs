using System;
using System.Threading;
using System.Threading.Tasks;
using THEIA.Core; // Убедись, что пространство имен правильное для твоего Brain
using THEIA.Services.Speech;

namespace THEIA;

class Program
{
    static async Task Main(string[] args)
    {
        var brain = new Brain();

        Console.WriteLine("Проверка всех систем . . .");
        Console.WriteLine("All Right");

        // Инициализируем детектор, указывая путь к русской ASR модели
        var detector = new WakeWordDetector("Data/Models/asr-ru");

        detector.WakeWordDetected += () =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🔊 ТЕИА: Да, да ... ?");
            Console.ResetColor();
        };

        detector.CommandRecognized += async (commandText) =>
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
        
        // Ждём нажатия Ctrl+C для корректного завершения
        var exitEvent = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            exitEvent.Set();
        };
        
        exitEvent.Wait();
        
        Console.WriteLine("\nОстанавливаем ТЕИА...");
        detector.Stop();
        detector.Dispose();
        Console.WriteLine("До свидания!");
    }
}