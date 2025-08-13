namespace Volte.Entities;

#nullable enable

public struct AppStatusEventArgs
{
    public const string AppStatusDefaultIcon = "fa-regular fa-message";
    
    public AppStatusEventArgs()
    {
    }

    public required string Status { get; init; }
    public string Icon { get; private init; } = FontAwesome.Message;
    public bool? IsWorkingStatus { get; private init; } = null;
    public TimeSpan? StatusExpiresAfter { get; private init; } = null;
    public LogSeverity? Severity { get; private init; } = null;


    public static event Action<AppStatusEventArgs> ChangeRequested
    {
        add => ChangeRequestedHandler.Add(value);
        remove => ChangeRequestedHandler.Remove(value);
    }

    private static readonly EventWithQueue<AppStatusEventArgs> ChangeRequestedHandler = new(fireEnqueuedEventsWhenFirstHandlerIsAdded: true);

    public static void SetAppStatus(
        string status,
        string icon = AppStatusDefaultIcon,
        bool? isWorkingStatus = null,
        TimeSpan? statusExpiresAfter = null,
        LogSeverity? severity = null
    )
    {
        if (VolteBot.IsHeadless) return;

        ChangeRequestedHandler.Call(new AppStatusEventArgs
        {
            Status = status,
            Icon = icon,
            IsWorkingStatus = isWorkingStatus,
            StatusExpiresAfter = statusExpiresAfter,
            Severity = severity
        });
    }
}