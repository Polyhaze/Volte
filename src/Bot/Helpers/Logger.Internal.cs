using Colorful;
using Sentry.Extensibility;

using Color = System.Drawing.Color;

namespace Volte.Helpers;

public static partial class Logger
{
    public class SentryTranslator : IDiagnosticLogger
    {
        public bool IsEnabled(SentryLevel logLevel) 
            => Version.IsDevelopment || logLevel is not SentryLevel.Debug;
            
        public void Log(SentryLevel logLevel, string message, Exception exception = null, params object[] args)
            => LogEventHandler.Call(new VolteLogEventArgs
            {
                Source = LogSource.Sentry,
                Severity = logLevel.ToSeverity(),
                Message = message.Format(args),
                Error = exception
            });
    }

    private static readonly string[] VolteAscii =
        Figlet.GetAscii("Volte").ConcreteValue.Split("\n", StringSplitOptions.RemoveEmptyEntries);

    static Logger() => FilePath.Logs.CreateAsDirectory();

    private static readonly Lock LogSync = new();

    internal static void PrintHeader()
    {
        if (!VolteBot.IsHeadless) return;

        Info(LogSource.Volte, CommandEventArgs.Separator.Trim());
        VolteAscii.ForEach(static ln => Info(LogSource.Volte, ln));
        Info(LogSource.Volte, CommandEventArgs.Separator.Trim());
    }

    private const string Side = "----------------------------------------------------------";
    private static bool _logFileNoticePrinted;

    internal static void LogFileRestartNotice()
    {
        if (!VolteBot.IsHeadless || _logFileNoticePrinted || Config.EnabledFeatures?.LogToFile is not true) return;

        GetLogFilePath(DateTime.Now).AppendAllText($"{Side}RESTARTING{Side}\n");

        _logFileNoticePrinted = true;
    }

    public static void Log(LogSeverity s, LogSource from, string message, Exception e = null, InvocationInfo caller = default) =>
        Log(new VolteLogEventArgs
        {
            Severity = s,
            Source = from,
            Message = message,
            Error = e,
            Invocation = caller
        });

    private static StringBuilder _logFileBuilder = new();

    private static void Execute(LogSeverity s, LogSource src, string message, Exception e, InvocationInfo caller)
    {
        if (IsDebugLoggingEnabled && caller.IsInitialized)
        {
            caller.ToString().IfPresent(debugInfoContent =>
            {
                // ReSharper disable once AccessToModifiedClosure
                Append(debugInfoContent, Color.Aquamarine, ref _logFileBuilder);
                Append(" |>  ", Color.Goldenrod, ref _logFileBuilder);
            });
        }
        
        var (color, value) = VerifySeverity(s);
        Append($"{value}:".P(), color);
        var dt = DateTime.Now.ToLocalTime();
        _logFileBuilder.Append($"[{dt.FormatDate()} | {dt.FormatFullTime()}] {value} - ");

        (color, value) = VerifySource(src);
        Append($"[{value}]".P(), color);
        _logFileBuilder.Append(string.Intern($"{value} -> "));

        if (!message.IsNullOrWhitespace())
            Append(message, Color.White, ref _logFileBuilder);

        if (e != null)
        {
            e.SentryCapture(scope => 
                scope.AddBreadcrumb("This exception might not have been thrown, and may not be important; it is merely being logged.")
            );
            
            Append(errorString(), Color.IndianRed, ref _logFileBuilder);

            string errorString()
                => Environment.NewLine + (e.Message.IsNullOrEmpty() ? "No message provided" : e.Message) +
                   Environment.NewLine + e.StackTrace;
        }

        if (Environment.NewLine != _logFileBuilder[^1].ToString())
        {
            Console.Write(Environment.NewLine);
            _logFileBuilder.AppendLine();
        }

        if (Config.EnabledFeatures?.LogToFile ?? false)
            GetLogFilePath(dt).AppendAllText(_logFileBuilder.ToString());

        _logFileBuilder.Length = 0;
    }

    public static FilePath GetLogFilePath(DateTime date) 
        => new FilePath("logs") / string.Intern($"{date.Year}-{date.Month}-{date.Day}.log");

    private static void Append(string m, Color c)
    {
        Console.ForegroundColor = c;
        Console.Write(m);
    }

    private static void Append(string m, Color c, ref StringBuilder sb)
    {
        Console.ForegroundColor = c;
        Console.Write(m);
        sb?.Append(m);
    }

    private static (Color Color, string Source) VerifySource(LogSource source) =>
        source switch
        {
            LogSource.Discord or LogSource.Gateway => (Color.RoyalBlue, "DISCORD"),
            LogSource.Volte => (Color.LawnGreen, "CORE"),
            LogSource.Service => (Color.Gold, "SERVICE"),
            LogSource.Module => (Color.LimeGreen, "MODULE"),
            LogSource.Rest => (Color.Red, "REST"),
            LogSource.Unknown => (Color.Fuchsia, "UNKNOWN"),
            LogSource.Sentry => (Color.Chartreuse, "SENTRY"),
            LogSource.UI => (Color.Crimson, "UI"),
            _ => throw new InvalidOperationException($"The specified LogSource {source} is invalid.")
        };


    private static (Color Color, string Level) VerifySeverity(LogSeverity severity) =>
        severity switch
        {
            LogSeverity.Critical => (Color.Maroon, "CRITICAL"),
            LogSeverity.Error => (Color.DarkRed, "ERROR"),
            LogSeverity.Warning => (Color.Yellow, "WARN"),
            LogSeverity.Info => (Color.SpringGreen, "INFO"),
            LogSeverity.Verbose => (Color.Pink, "VERBOSE"),
            LogSeverity.Debug => (Color.SandyBrown, "DEBUG"),
            _ => throw new InvalidOperationException($"The specified LogSeverity ({severity}) is invalid.")
        };

    public static string P(this string input, int padding = 10) => string.Intern(input.PadRight(padding));

    public static LogSeverity ToSeverity(this SentryLevel sentryLevel) =>
        sentryLevel switch
        {
            SentryLevel.Debug => LogSeverity.Debug,
            SentryLevel.Info => LogSeverity.Info,
            SentryLevel.Warning => LogSeverity.Warning,
            SentryLevel.Error => LogSeverity.Error,
            SentryLevel.Fatal => LogSeverity.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(sentryLevel), sentryLevel, null)
        };

    public static SentryLevel ToSentryLevel(this LogSeverity severity) =>
        severity switch
        {
            LogSeverity.Critical => SentryLevel.Fatal,
            LogSeverity.Error => SentryLevel.Error,
            LogSeverity.Warning => SentryLevel.Warning,
            LogSeverity.Info => SentryLevel.Info,
            LogSeverity.Verbose => SentryLevel.Info,
            LogSeverity.Debug => SentryLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
}