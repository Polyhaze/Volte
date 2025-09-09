using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using Volte.UI.Avalonia.Models;
using Volte.UI.Avalonia.Views;
using Volte.UI.Helpers;

namespace Volte.UI.Avalonia.ViewModels;

public partial class LogsViewModel : BaseModel
{
    private const byte MaxLogsInMemory = 200;

    private readonly Lock _lock = new();

    public required LogsView? View { get; init; }

    public required int LogsClearAmount { get; init; }

    [ObservableProperty] private ObservableCollection<VolteLog> _logs = [];

    [ObservableProperty] private VolteLog? _selected;

    public LogsViewModel() => Logger.Event += Receive;

    ~LogsViewModel() => Logger.Event -= Receive;

    public static void UnregisterHandler() 
        => Logger.Event -= PageManager.Shared.GetViewModel<LogsViewModel>().Receive;

    private void Receive(VolteLogEventArgs eventArgs)
    {
        if (eventArgs is
            {
                Source: LogSource.Sentry,
                Severity: LogSeverity.Debug,
                Message: not null
            }
           ) return; //sentry debug messages are huge and break the log view entirely.

        lock (_lock)
        {
            IfNeededRemoveLast(LogsClearAmount);

            Logs.Add(new VolteLog(eventArgs));
        }
        
        Lambda.Try(() => Dispatcher.UIThread.Invoke(() => View?.Viewer?.ScrollToEnd()));

        if (eventArgs.Error is not { } err) return;

        VolteApp.NotifyError(err);
        err.SentryCapture(scope =>
            scope.AddBreadcrumb(
                "This exception might not have been thrown, and may not be important; it is merely being logged.")
        );
    }

    private void IfNeededRemoveLast(int amount)
    {
        if (Logs.Count < MaxLogsInMemory) return;

        Logs.Index()
            .OrderByDescending(static x => x.Item.Date)
            .TakeLast(amount)
            .ForEach(toRemove => Logs.RemoveAt(toRemove.Index));
    }
}