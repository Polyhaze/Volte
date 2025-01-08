namespace Volte.Entities;

#nullable enable

public abstract class NotificationEventArgs
{
    public byte RawType { get; protected init; }

    public Gommon.Optional<TimeSpan> Expiration { get; protected init; }

    public Action? Clicked { get; protected init; }

    public Action? Closed { get; protected init; }


    /// <summary>
    ///     Subscribers to this event should pattern match the event args with the known types:
    ///     <see cref="CustomNotificationEventArgs"/>, <see cref="ErrorNotificationEventArgs"/>
    ///     <br/>
    ///     The base abstract class contains only common properties, not the full notification data (title &amp; message). <br/>
    ///     The Error type contains just an exception, for the receiver to format however they like.
    /// </summary>
    public static event Action<NotificationEventArgs> NotificationSent
    {
        add => NotificationSentHandler.Add(value);
        remove => NotificationSentHandler.Remove(value);
    }

    protected static readonly Event<NotificationEventArgs> NotificationSentHandler = new();
}

#region Implementations

public sealed class CustomNotificationEventArgs : NotificationEventArgs
{
    private CustomNotificationEventArgs(byte rawType) 
        => RawType = rawType;

    public required string Title { get; init; }
    public required string Message { get; init; }

    public static void NotifyInfo(
        string title,
        string message,
        Gommon.Optional<TimeSpan> expiration = default,
        Action? onClick = null,
        Action? onClose = null
    )
        => NotificationSentHandler.Call(new CustomNotificationEventArgs(0)
        {
            Title = title,
            Message = message,
            Expiration = expiration,
            Clicked = onClick,
            Closed = onClose
        });

    public static void NotifySuccess(
        string title,
        string message,
        Gommon.Optional<TimeSpan> expiration = default,
        Action? onClick = null,
        Action? onClose = null
    )
        => NotificationSentHandler.Call(new CustomNotificationEventArgs(1)
        {
            Title = title,
            Message = message,
            Expiration = expiration,
            Clicked = onClick,
            Closed = onClose
        });

    public static void NotifyWarning(
        string title,
        string message,
        Gommon.Optional<TimeSpan> expiration = default,
        Action? onClick = null,
        Action? onClose = null
    )
        => NotificationSentHandler.Call(new CustomNotificationEventArgs(2)
        {
            Title = title,
            Message = message,
            Expiration = expiration,
            Clicked = onClick,
            Closed = onClose
        });

    public static void NotifyError(
        string title,
        string message,
        Gommon.Optional<TimeSpan> expiration = default,
        Action? onClick = null,
        Action? onClose = null
    )
        => NotificationSentHandler.Call(new CustomNotificationEventArgs(3)
        {
            Title = title,
            Message = message,
            Expiration = expiration,
            Clicked = onClick,
            Closed = onClose
        });
}

public sealed class ErrorNotificationEventArgs : NotificationEventArgs
{
    private ErrorNotificationEventArgs()
    {
        RawType = 3;
    }

    public Exception Error { get; private init; } = null!;

    public static void NotifyError(Exception ex, Gommon.Optional<TimeSpan> expiration = default, Action? onClick = null)
        => NotificationSentHandler.Call(new ErrorNotificationEventArgs
        {
            Error = ex,
            Expiration = expiration,
            Clicked = onClick
        });
}

#endregion Implementations