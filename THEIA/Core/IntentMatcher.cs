using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using THEIA.Commands;
using THEIA.Ui.UiManager;

namespace THEIA.Core;

class IntentMatcher
{
    private readonly Dictionary<string, ActionCommand> _command = new()
    {
        //Системные команды
        ["открой браузер"] = ActionCommand.open_browser,
        ["открой ютуб"] = ActionCommand.open_youtube,
        ["выключи комп"] = ActionCommand.shutdown,
        ["найди"] = ActionCommand.search,

        //Информационные команды
        ["сколько время"] = ActionCommand.get_time,
        ["время"] = ActionCommand.get_time,
        ["день недели"] = ActionCommand.get_week,
        ["кто ты"] = ActionCommand.WhoAmI,
        ["ты кто"] = ActionCommand.WhoAmI,

        //EXIT
        ["конец связи"] = ActionCommand.exit,
        ["пока"] = ActionCommand.exit,
        ["выход"] = ActionCommand.sleep,
        ["погоди"] = ActionCommand.sleep,
        ["до скорого"] = ActionCommand.sleep,
        ["стой"] = ActionCommand.sleep,
        ["стоп"] = ActionCommand.sleep

    };

    private readonly Dictionary<string, string> _synonyms = new()
    {
        ["гугл"] = "браузер",
        ["ютюб"] = "ютуб",
        ["интернет"] = "браузер",
        ["инет"] = "браузер",

        ["найти"] = "найди",
        ["поищи"] = "найди",
        ["загугли"] = "найди",
        ["поищи"] = "найди",

        ["компьютер"] = "комп",
        ["пк"] = "комп"
    };

    // Кэш N-грамм для команд (чтобы не пересчитывать при каждом запросе)
    private readonly Dictionary<string, HashSet<string>> _commandNgramsCache;
    
    // Настройки N-грамм
    private readonly int _ngramSize = 3;           // Размер N-граммы (3 = триграммы)
    private readonly double _threshold = 0.6;     // Порог срабатывания (60% N-грамм команды должно совпасть)

    public IntentMatcher()
    {
        // Предварительно вычисляем N-граммы для всех команд при старте
        _commandNgramsCache = new Dictionary<string, HashSet<string>>();
        foreach (var commandKey in _command.Keys)
        {
            _commandNgramsCache[commandKey] = GenerateNgrams(commandKey);
        }
    }

    public ActionCommand? Match(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) return null;

        // 1. Нормализация и замена синонимов
        var normalized = NormalizeText(userInput);
        normalized = ReplaceSynonyms(normalized);

        // 2. Генерация N-грамм из фразы пользователя
        var inputNgrams = GenerateNgrams(normalized);

        // 3. Поиск команды с максимальным покрытием
        ActionCommand? bestMatch = null;
        string? bestMatchName = null;
        double bestScore = 0;

        foreach (var kvp in _commandNgramsCache)
        {
            double score = CalculateCoverage(inputNgrams, kvp.Value);

            if (score > bestScore)
            {
                bestScore = score;
                bestMatch = _command[kvp.Key];
                bestMatchName = kvp.Key;
            }
        }

        // 4. Проверка порога
        if (bestScore >= _threshold && bestMatch != null)
        {
            UiManager.Print($"(Debugger) Распознано: '{bestMatchName}' (Покрытие: {bestScore:P0})", "grey",EmojiCategory.funny);
            
            return bestMatch;
        }

        UiManager.Print($"(Debugger) Команда не распознана. Лучшее совпадение: '{bestMatchName}' ({bestScore:P0})", "grey");
        return null;
    }

    /// Замена синонимов в тексте перед поиском
    private string ReplaceSynonyms(string text)
    {
        foreach (var synonym in _synonyms)
        {
            // Заменяем целые слова (чтобы "инет" не заменилось внутри "планета")
            text = Regex.Replace(text, $@"\b{Regex.Escape(synonym.Key)}\b", synonym.Value);
        }
        return text;
    }

    /// Покрытие: какой процент N-грамм команды нашёлся во фразе пользователя.
    /// Это идеально для голосового ввода, где фраза длиннее команды.
    private double CalculateCoverage(HashSet<string> inputNgrams, HashSet<string> commandNgrams)
    {
        if (commandNgrams.Count == 0) return 0;

        int intersectionCount = inputNgrams.Intersect(commandNgrams).Count();
        return (double)intersectionCount / commandNgrams.Count;
    }

    /// Генерация множества N-грамм из строки
    /// 
    private HashSet<string> GenerateNgrams(string text)
    {
        var ngrams = new HashSet<string>();

        if (text.Length <= _ngramSize)
        {
            ngrams.Add(text);
            return ngrams;
        }

        for (int i = 0; i <= text.Length - _ngramSize; i++)
        {
            ngrams.Add(text.Substring(i, _ngramSize));
        }

        return ngrams;
    }


    /// Улучшенная нормализация через Regex (чище, чем куча Replace)

    private string NormalizeText(string text)
    {
        return Regex.Replace(text.ToLower(), @"[^\w\s]", "").Trim();
    }
}


/// АЛГОРИТМ N-граммирование был сгенерирован с помощью нейросети QWEN 3.7+