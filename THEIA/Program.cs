using System;
using System.Threading;
using System.Threading.Tasks;
using THEIA.Core; // Убедись, что пространство имен правильное для твоего Brain
using THEIA.Services.Speech;
using THEIA.Ui.UiManager;

namespace THEIA;

class Program
{
    static async Task Main(string[] args)
    {

        UiManager.Print("Проверка систем...", color :"yellow", ChooseEmoji:EmojiCategory.funny );

        // Инициализируем детектор, указывая путь к русской ASR модели
        var detector = new WakeWordDetector("Data/Models/asr-ru");
        var brain = new Brain(detector);

        detector.WakeWordDetected += () =>
        {
            UiManager.Print("Да, да ... ?", color : "green");
        };

        detector.CommandRecognized += async (commandText) =>
        {
            UiManager.Print($"Вы услышали: {commandText}", color : "white");
            
            // Передаём команду в мозг
            var response = await brain.ProcessCommandAsync(commandText);
            
            UiManager.Print(response?? "Не удалось получить ответ", color:"green", ChooseEmoji: EmojiCategory.funny);
        };


        UiManager.Print("Все системы в норме", color:"green", EmojiCategory.success);
        UiManager.Print("=== Тейя запускается ===", color:"green", EmojiCategory.success, "bold");
        UiManager.Print("Скажи 'тея' чтобы разбудить...", color:"green", EmojiCategory.success);

        
        detector.Start();
        
        // Ждём нажатия Ctrl+C для корректного завершения на Линукс 
        var exitEvent = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            exitEvent.Set();
        };
        
        exitEvent.Wait();
        
        UiManager.Print("Останавливаем ТЕЙЯ...", color:"red", EmojiCategory.error, "bold");
        detector.Stop();
        detector.Dispose(); 
        UiManager.Print("Останавливаем ТЕЙЯ...", color:"red", EmojiCategory.error, "bold");    
        }
}