using Volte.Helpers;

namespace Volte.UI;

public class VolteManager
{
    private static Task? _botTask;

    public static CancellationTokenSource? Cts { get; private set; }

    public static void Start()
    {
        if (VolteBot.Client is not null && Cts is not null) return;

        Cts = new();
        
        _botTask = Task.Run(async () => await VolteBot.LoginAsync(Cts), Cts.Token);
    }
    
    public static async Task<int> StartWait()
    {
        if (VolteBot.IsHeadless)
            Logger.OutputLogToStandardOut();
        
        Start();
        await _botTask!;
        return 0;
    }
    
    public static void Stop()
    {
        if (VolteBot.Client is null && Cts is null) return;
        
        Cts!.Cancel();
        _botTask = null;

        Cts = null;
    }
    
    public static string GetConnectionState()
        => VolteBot.Client is null
            ? "Disconnected"
            : Enum.GetName(VolteBot.Client.ConnectionState) ?? "Disconnected";
}