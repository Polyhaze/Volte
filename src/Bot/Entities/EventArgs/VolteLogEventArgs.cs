namespace Volte.Entities;

#nullable enable

public struct VolteLogEventArgs
{
    public VolteLogEventArgs() { }

    public VolteLogEventArgs(DiscordLogMessage logMessage)
    {
        Severity = logMessage.Severity;
        Source = LogSources.Parse(logMessage.Source);
        Message = logMessage.Message;
        Error = logMessage.Exception;
    }

    public LogSeverity Severity { get; init; }
    public LogSource Source { get; init; }
    public string? Message { get; init; }
    public Exception? Error { get; init; }
    public InvocationInfo Invocation { get; init; } = default;
}