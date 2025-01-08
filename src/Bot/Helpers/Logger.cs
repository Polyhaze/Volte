using System.IO;
using System.Runtime.CompilerServices;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Helpers;

public static partial class Logger
{
    public static event Action<VolteLogEventArgs> Event
    {
        add => LogEventHandler.Add(value);
        remove => LogEventHandler.Remove(value);
    }

    private static readonly Event<VolteLogEventArgs> LogEventHandler = new(enableHandlerlessQueue: true);

    public static void Log(VolteLogEventArgs eventArgs) => LogEventHandler.CallHandlers(eventArgs);

    public static bool IsDebugLoggingEnabled => Config.DebugEnabled || Version.IsDevelopment;

    #region Logger methods with invocation info

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Debug"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Debug(LogSource src, string message, InvocationInfo caller)
        => Log(LogSeverity.Debug, src, message, null, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Info"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Info(LogSource src, string message, InvocationInfo caller)
        => Log(LogSeverity.Info, src, message, null, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Error(LogSource src, string message, InvocationInfo caller, Exception e = null)
        => Log(LogSeverity.Error, src, message, e, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Critical(LogSource src, string message, InvocationInfo caller, Exception e = null)
        => Log(LogSeverity.Critical, src, message, e, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Warn(LogSource src, string message, InvocationInfo caller, Exception e = null)
        => Log(LogSeverity.Warning, src, message, e, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Verbose"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Verbose(LogSource src, string message, InvocationInfo caller)
        => Log(LogSeverity.Verbose, src, message, null, caller);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="e"/> exception.
    ///     This method calls <see cref="SentrySdk"/>'s CaptureException, so it is logged to Sentry.
    /// </summary>
    /// <param name="e">Exception to print.</param>
    /// <param name="src">Source to print the message from.</param>
    public static void Error(Exception e, InvocationInfo caller, LogSource src = LogSource.Volte)
        => Error(src, string.Empty, caller, e);

    #endregion

    #region Normal logger methods

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Debug"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Debug(LogSource src, string message)
        => Log(LogSeverity.Debug, src, message);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Info"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Info(LogSource src, string message)
        => Log(LogSeverity.Info, src, message);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Error(LogSource src, string message, Exception e = null)
        => Log(LogSeverity.Error, src, message, e);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Critical(LogSource src, string message, Exception e = null)
        => Log(LogSeverity.Critical, src, message, e);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    /// <param name="e">Optional Exception to print.</param>
    public static void Warn(LogSource src, string message, Exception e = null)
        => Log(LogSeverity.Warning, src, message, e);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Verbose"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
    /// </summary>
    /// <param name="src">Source to print the message from.</param>
    /// <param name="message">Message to print.</param>
    public static void Verbose(LogSource src, string message)
        => Log(LogSeverity.Verbose, src, message);

    /// <summary>
    ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="e"/> exception.
    ///     This method calls <see cref="SentrySdk"/>'s CaptureException, so it is logged to Sentry.
    /// </summary>
    /// <param name="e">Exception to print.</param>
    public static void Error(Exception e)
        => Error(LogSource.Volte, string.Empty, e);

    #endregion

    private static readonly string[] _ignoredLogMessages =
    [
        "You're using the GuildPresences intent without listening to the PresenceUpdate event",
        "application_command",
        "unknown dispatch"
    ];

    public static void Listen(DiscordSocketClient client) =>
        client.Log += m =>
        {
            if (!m.Message.ContainsAnyIgnoreCase(_ignoredLogMessages))
                Log(new VolteLogEventArgs(m));

            return Task.CompletedTask;
        };

    public static void OutputLogToStandardOut()
    {
        LogEventHandler.Clear();
        Event += logEvent => LogSync.Lock(() => Execute(logEvent.Severity, logEvent.Source, logEvent.Message, logEvent.Error, logEvent.Invocation));
    }
}

public readonly struct InvocationInfo
{
    /// <summary>
    ///     Creates an <see cref="InvocationInfo"/> with information about the current source file, line, and member name.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific line in the specific member, in the source file in which it is created.</returns>
    public static InvocationInfo Here(
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!
    ) => new(sourceLocation, lineNumber, callerName);

    /// <summary>
    ///     Creates a partial <see cref="InvocationInfo"/> with information about the current source file and line.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific line in the source file in which it is created.</returns>
    public static InvocationInfo CurrentFileLocation(
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default
    ) => new(sourceLocation, lineNumber);

    /// <summary>
    ///     Creates a partial <see cref="InvocationInfo"/> with only information about the current member name.
    ///     Do not provide the arguments!
    /// </summary>
    /// <remarks>Mostly used in the logger.</remarks>
    /// <returns>An <see cref="InvocationInfo"/> referencing the specific C# source member it was created in.</returns>
    public static InvocationInfo CurrentMember([CallerMemberName] string callerName = default!) => new(callerName);
    
    public bool IsInitialized { get; }

    public string FilePath { get; }
    public int LineInFile { get; }
    public string CallerName { get; }

    public (bool Full, bool FileLoc, bool CallerOnly) Type { get; }

    // ReSharper disable once UnusedMember.Global
    // this is used by the default keyword
    public InvocationInfo()
    {
        IsInitialized = false;
        Type = (false, false, false);
    }

    public InvocationInfo(string filePath, int line, string caller)
    {
        IsInitialized = true;
        Type = (true, false, false);

        FilePath = filePath;
        LineInFile = line;
        CallerName = caller;
    }

    public InvocationInfo(string filePath, int line)
    {
        IsInitialized = true;
        Type = (false, true, false);

        FilePath = filePath;
        LineInFile = line;
    }

    public InvocationInfo(string caller)
    {
        IsInitialized = true;
        Type = (false, false, true);

        CallerName = caller;
    }

    public new Gommon.Optional<string> ToString() 
        => Type switch
        {
            { Full: true } => $"{CallerName}:{this.GetSourceFileName()}:{LineInFile}",
            { CallerOnly: true } => CallerName,
            { FileLoc: true } => $"{this.GetSourceFileName()}:{LineInFile}",
            _ => (Gommon.Optional<string>) default 
            //casting to ensure default branch returns a default optional and not an optional with a null string value
        };
}

public static class InvocationInfoExt
{
    public static string GetSourceFileName(this InvocationInfo invocation)
        => invocation.FilePath[
            (invocation.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..
        ];


}