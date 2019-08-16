using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Services
{
    public sealed class LoggingService : VolteEventService
    {
        private const string LogFile = "data/Volte.log";
        private readonly object _lock = new object();

        public override Task DoAsync(EventArgs args)
        {
            Log(args.Cast<LogEventArgs>());
            return Task.CompletedTask;
        }

        private void Log(LogEventArgs args) =>
            Log(args.LogMessage.Internal.Severity, args.LogMessage.Internal.Source,
                args.LogMessage.Internal.Message, args.LogMessage.Internal.Exception);

        internal void PrintVersion() => Info(LogSource.Volte, $"Currently running Volte V{Version.FullVersion}.");

        private void Log(LogSeverity s, LogSource from, string message, Exception e = null)
        {
            lock (_lock)
            {
                if (s is LogSeverity.Debug)
                {
                    if (from is LogSource.Volte && !Config.EnableDebugLogging) return;
                }

                Execute(s, from, message, e);
            }
        }

        /// <summary>
        ///     Prints a <see cref="LogSeverity.Debug"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Debug(LogSource src, string message) 
            => Log(LogSeverity.Debug, src, message);

        /// <summary>
        ///     Prints a <see cref="LogSeverity.Info"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Info(LogSource src, string message)
            => Log(LogSeverity.Info, src, message);

        /// <summary>
        ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Error(LogSource src, string message, Exception e = null)
            => Log(LogSeverity.Error, src, message, e);
        /// <summary>
        ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Critical(LogSource src, string message, Exception e = null)
            => Log(LogSeverity.Critical, src, message, e);

        /// <summary>
        ///     Prints a <see cref="LogSeverity.Critical"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message, with the specified <paramref name="e"/> exception if provided.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        /// <param name="e">Optional Exception to print.</param>
        public void Warn(LogSource src, string message, Exception e = null)
            => Log(LogSeverity.Warning, src, message, e);
        /// <summary>
        ///     Prints a <see cref="LogSeverity.Verbose"/> message to the console from the specified <paramref name="src"/> source, with the given <paramref name="message"/> message.
        /// </summary>
        /// <param name="src">Source to print the message from.</param>
        /// <param name="message">Message to print.</param>
        public void Verbose(LogSource src, string message)
            => Log(LogSeverity.Verbose, src, message);

        /// <summary>
        ///     Prints a <see cref="LogSeverity.Error"/> message to the console from the specified <paramref name="e"/> exception.
        /// </summary>
        /// <param name="e">Exception to print.</param>
        public void LogException(Exception e)
            => Log(LogSeverity.Error, LogSource.Volte, string.Empty, e);

        private void Execute(LogSeverity s, LogSource src, string message, Exception e)
        {
            var content = new StringBuilder();
            var (color, value) = VerifySeverity(s);
            Append($"{value} -> ", color);
            var dto = DateTimeOffset.UtcNow;
            content.Append($"[{dto.FormatDate()} | {dto.FormatFullTime()}] {value} -> ");

            (color, value) = VerifySource(src);
            Append($"{value} -> ", color);
            content.Append($"{value} -> ");

            if (!message.IsNullOrWhitespace())
            {
                Append(message, Color.White);
                content.Append(message);
            }

            if (e != null)
            {
                var toWrite = $"{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}";
                Append(toWrite, Color.IndianRed);
                content.Append(toWrite);
            }

            Console.Write(Environment.NewLine);
            content.AppendLine();
            if (Config.EnabledFeatures.LogToFile)
            {
                File.AppendAllText(LogFile, content.ToString());
            }
        }

        private void Append(string m, Color c)
        {
            Console.ForegroundColor = c;
            Console.Write(m);
        }

        private (Color Color, string Source) VerifySource(LogSource source) =>
            source switch
                {
                LogSource.Discord => (Color.RoyalBlue, "DSCD"),
                LogSource.Gateway => (Color.RoyalBlue, "DSCD"),
                LogSource.Volte => (Color.Crimson, "CORE"),
                LogSource.Service => (Color.Gold, "SERV"),
                LogSource.Module => (Color.LimeGreen, "MDLE"),
                LogSource.Rest => (Color.Tomato, "REST"),
                LogSource.Unknown => (Color.Teal, "UNKN"),
                _ => throw new ArgumentNullException(nameof(source), "source cannot be null")
                };


        private (Color Color, string Level) VerifySeverity(LogSeverity severity) =>
            severity switch
                {
                LogSeverity.Critical => (Color.Maroon, "CRIT"),
                LogSeverity.Error => (Color.DarkRed, "EROR"),
                LogSeverity.Warning => (Color.Yellow, "WARN"),
                LogSeverity.Info => (Color.SpringGreen, "INFO"),
                LogSeverity.Verbose => (Color.Pink, "VRBS"),
                LogSeverity.Debug => (Color.SandyBrown, "DEBG"),
                _ => throw new ArgumentNullException(nameof(severity), "severity cannot be null")
                };
    }
}