using System.IO;

namespace Volte.Entities;

public class CalledCommandsInfo
{
    public static ulong ThisSessionSuccess { get; private set; }
    public static ulong ThisSessionFailed { get; private set; }
    
    public static ulong Successes => Instance.Successful;
    public static ulong Failures => Instance.Failed;

    public static ulong Sum => Instance.Total;

    public static readonly FilePath CalledCommandsFile = FilePath.Data / "commandstats.bin";

    private static bool _isInitialized;
    private static bool _persistenceStarted;
    // ReSharper disable once InconsistentNaming
    private static readonly CalledCommandsInfo _instance = new();

    public static CalledCommandsInfo Instance
    {
        get
        {
            if (!_isInitialized)
            {
                Load();
                _isInitialized = true;
            }

            return _instance;
        }
    }

    public ulong Successful { get; set; }
    public ulong Failed { get; set; }

    public ulong Total => Successful + Failed;

    public void Write(Stream stream)
    {
        using var bw = new BinaryWriter(stream);
        bw.Write(Successful);
        bw.Write(Failed);
    }

    public void Read(Stream stream)
    {
        using var br = new BinaryReader(stream);
        
        if (!stream.LengthEquals(sizeof(ulong) * 2) /* 16 */) return;

        Successful = br.ReadUInt64();
        Failed = br.ReadUInt64();
    }

    public static void Load()
    {
        if (!CalledCommandsFile.ExistsAsFile)
        {
            CalledCommandsFile.CreateAsFile();
            Save();
        }
        else
        {
            using var readStream = CalledCommandsFile.OpenRead();
            _instance.Read(readStream);
        }
    }

    public static void Save()
    {
        using var fileStream = CalledCommandsFile.OpenWrite();
        _instance.Write(fileStream);
    }

    public static void UpdateSaved(MessageService messageService)
    {
        Instance.Successful += messageService.UnsavedSuccessfulCommandCalls;
        Instance.Failed += messageService.UnsavedFailedCommandCalls;
        messageService.ResetCalledCommands();
        Save();
    }

    public static void StartPersistence(IServiceProvider provider, TimeSpan saveEvery)
    {
        if (_persistenceStarted) return;
        
        ExecuteBackgroundAsync(async () =>
        {
            var ticker = new PeriodicTimer(saveEvery);
            while (await ticker.WaitForNextTickAsync(provider.Get<CancellationTokenSource>().Token))
            {
                Debug(LogSource.Service, "Saving command stats.");
                var ms = provider.Get<MessageService>();
                ThisSessionSuccess += ms.UnsavedSuccessfulCommandCalls;
                ThisSessionFailed += ms.UnsavedFailedCommandCalls;
                UpdateSaved(ms);
            }
        });

        _persistenceStarted = true;
    }
}