using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SherpaOnnx;

namespace THEIA.Services.Speech;

public class WakeWordDetector : IDisposable
{
    private const int SampleRate = 16000;
    private const int DialogTimeoutSeconds = 25;
    
    private readonly OnlineRecognizer _recognizer;
    private OnlineStream _stream;
    
    private Process? _arecordProcess;
    private Thread? _audioThread;
    private readonly object _stateLock = new();
    
    private volatile bool _isAwake = false;
    private volatile bool _disposed = false;
    private CancellationTokenSource? _commandTimeout;
    
    public event Action<string>? CommandRecognized;
    public event Action? WakeWordDetected;
    
    public WakeWordDetector(string modelPath)
    {
        // 1. Настраиваем русскую ASR модель
        var config = new OnlineRecognizerConfig();
        config.FeatConfig.SampleRate = SampleRate;
        config.FeatConfig.FeatureDim = 80;
        
        // ⚠️ ВАЖНО: Проверь через ls Data/Models/asr-ru, как точно называются твои файлы!
        // Если там encoder-epoch-99-avg-1.onnx, замени "encoder.onnx" на это полное имя.
        config.ModelConfig.Transducer.Encoder = Path.Combine(modelPath, "encoder.onnx");
        config.ModelConfig.Transducer.Decoder = Path.Combine(modelPath, "decoder.onnx");
        config.ModelConfig.Transducer.Joiner = Path.Combine(modelPath, "joiner.onnx");
        config.ModelConfig.Tokens = Path.Combine(modelPath, "tokens.txt");
        
        config.ModelConfig.Provider = "cpu";
        config.ModelConfig.NumThreads = 2;
        config.ModelConfig.Debug = 0;
        
        // Настройки окончания фразы (пауза)
        config.EnableEndpoint = 1;
        config.Rule1MinTrailingSilence = 1.5f; // Тишина перед концом фразы
        config.Rule2MinTrailingSilence = 0.7f; // Тишина после короткой фразы
        config.Rule3MinUtteranceLength = 15f;  // Минимальная длина фразы
        
        _recognizer = new OnlineRecognizer(config);
        _stream = _recognizer.CreateStream();
    }
    
    public void Start()
    {
        // 2. Запускаем arecord с явным указанием USB-микрофона (card 0, device 0)
        _arecordProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "arecord",
                // -D plughw:0,0 гарантирует, что мы пишем с правильного микрофона
                Arguments = $"-D plughw:0,0 -f S16_LE -r {SampleRate} -c 1 -t raw -q",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        _arecordProcess.Start();
        
        _audioThread = new Thread(ProcessAudio);
        _audioThread.Start();
        
        Console.WriteLine("💤 ТЕИА спит...");
    }
    
    private void ProcessAudio()
    {
        // 3200 байт = 1600 сэмплов = 100 мс аудио при 16kHz и 16-бит
        var buffer = new byte[3200]; 
        
        while (!_disposed && _arecordProcess != null && !_arecordProcess.HasExited)
        {
            int bytesRead = _arecordProcess.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                ProcessAudioChunk(buffer, bytesRead);
            }
        }
    }
    
    private void ProcessAudioChunk(byte[] buffer, int bytesRead)
    {
        if (_disposed) return;
        
        lock (_stateLock)
        {
            // Конвертируем 16-bit PCM (byte[]) в float[] [-1.0, 1.0]
            int numSamples = bytesRead / 2;
            float[] samples = new float[numSamples];
            
            for (int i = 0; i < numSamples; i++)
            {
                short sample = (short)(buffer[i * 2] | (buffer[i * 2 + 1] << 8));
                samples[i] = sample / 32768.0f;
            }
            
            // Отправляем аудио в модель
            _stream.AcceptWaveform(SampleRate, samples);
            
            // Декодируем, пока есть данные
            while (_recognizer.IsReady(_stream))
            {
                _recognizer.Decode(_stream);
            }
            
            // Получаем текущий лучший результат распознавания
            var result = _recognizer.GetResult(_stream);
            string currentText = result.Text.Trim();

            if (!string.IsNullOrEmpty(currentText))
            {
                if (!_isAwake)
                {
                    // Режим сна: ищем слово пробуждения в текущем тексте
                    if (currentText.Contains("тея") || 
                        currentText.Contains("ты я") || 
                        currentText.Contains("т я") || 
                        currentText.Contains("ты") ||
                        currentText.Contains("тебя") ||
                        currentText.Contains("эй") ||
                        currentText.Contains("алло") ||
                        currentText.Contains("т")  )

                    {
                        OnWakeWord();
                    }
                }
            }

            // 🔥 ГЛАВНОЕ: Отправляем команду в ИИ ТОЛЬКО когда фраза закончена (наступила пауза)
            if (_recognizer.IsEndpoint(_stream))
            {
                if (_isAwake)
                {
                    string finalText = result.Text.Trim();
                    
                    // Отправляем в Brain только если текст не пустой
                    if (!string.IsNullOrEmpty(finalText))
                    {
                        // Дополнительно фильтруем, чтобы само слово пробуждения не улетало как команда
                        if (!finalText.Equals("тея", StringComparison.OrdinalIgnoreCase) &&
                            !finalText.Equals("ты", StringComparison.OrdinalIgnoreCase))
                        {
                            CommandRecognized?.Invoke(finalText);
                        }
                    }
                    
                    ResetCommandTimeout();
                }
                
                // Сбрасываем поток для следующей фразы
                _stream.Dispose();
                _stream = _recognizer.CreateStream();
            }
        }
    }
    
    private void OnWakeWord()
    {
        if (_isAwake) return;
        _isAwake = true;
        
        // Сбрасываем поток, чтобы хвост слова "тея" не попал в команду
        _stream.Dispose();
        _stream = _recognizer.CreateStream();
        
        WakeWordDetected?.Invoke();
        ResetCommandTimeout();
    }
    
    private async void ResetCommandTimeout()
    {
        _commandTimeout?.Cancel();
        _commandTimeout?.Dispose();
        _commandTimeout = new CancellationTokenSource();
        
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(DialogTimeoutSeconds), _commandTimeout.Token);
            _isAwake = false;
            Console.WriteLine("⏰ Таймаут. Засыпаю...");
        }
        catch (TaskCanceledException) { }
    }
    public void Sleep()
    {
        _isAwake = false;
        Console.WriteLine("До скорого! Засыпаю . . .");
    }
    
    public void Stop()
    {
        _arecordProcess?.Kill();
        _audioThread?.Join(1000);
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        Stop();
        _stream?.Dispose();
        _recognizer?.Dispose();
        _commandTimeout?.Cancel();
        _commandTimeout?.Dispose();
    }
}