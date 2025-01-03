using CommunityToolkit.Mvvm.ComponentModel;
using Discord;
using Gommon;
using Humanizer;
using Volte.Entities;
using Volte.Helpers;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.UI.Helpers;

public partial class AppStatus : ObservableObject
{
    public static AppStatus Shared { get; } = new();

    // ReSharper disable once NotAccessedField.Local
    private readonly Timer _timer;

    static AppStatus()
    {
        AppStatusEventArgs.ChangeRequested += evt =>
            Set(evt.Status, evt.Icon, evt.IsWorkingStatus, evt.StatusExpiresAfter, evt.Severity);
    }

    private AppStatus()
    {
        _timer = new(_ =>
        {
            if (!IsWorking) return;
            
            Status += '.';
            Status = Status.Replace(".....", string.Empty);
        }, null, dueTime: 0.Seconds(), period: 0.3.Seconds());
    }

    [ObservableProperty] private string _status = "Ready";

    [ObservableProperty] private string _icon = FontAwesome.Message;

    [ObservableProperty] private bool _isWorking;

    public override string ToString() => Status;

    /// <summary>
    /// Resets the modal status to "Ready"
    /// </summary>
    public static void Reset() => Set("Ready");

    /// <summary>
    /// Sets a custom message and icon to the global status
    /// </summary>
    /// <param name="status">The message to use in the status modal.</param>
    /// <param name="icon">The fontawesome icon name and type to use in the status modal.</param>
    /// <param name="isWorkingStatus">If the message should have an animated ellipsis to indicate background work being done.</param>
    /// <param name="statusExpiresAfter">Reset the status message after a set amount of time.</param>
    /// <param name="logSeverity">The severity to print the message to the log as.</param>
    public static void Set(
        string status,
        string icon = AppStatusEventArgs.AppStatusDefaultIcon,
        bool? isWorkingStatus = null,
        TimeSpan? statusExpiresAfter = null,
        LogSeverity? logSeverity = null
    ) 
    {
        var isResetStatus = status.EqualsIgnoreCase("ready");

        Shared.Status = status;
        Shared.IsWorking = isWorkingStatus ?? !isResetStatus;
        Shared.Icon = icon;

        if (statusExpiresAfter is not null)
        {
            System.Timers.Timer resetTimer = new()
            {
                AutoReset = false,
                Interval = statusExpiresAfter.Value.TotalMilliseconds
            };

            resetTimer.Elapsed += (_, _) =>
            {
                if (Shared.Status == status)
                    Reset();

                resetTimer.Dispose();
            };

            resetTimer.Start();
        }

        if (!isResetStatus && logSeverity is { } sev)
            Logger.Log(sev, LogSource.UI, status);
    }
}