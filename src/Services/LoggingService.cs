using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Services
{
    [Service("Logging", "The main Service used to handle logging to the bot's console.")]
    public sealed class LoggingService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        internal Task LogAsync(LogEventArgs args) =>
            LogAsync(args.LogMessage.Internal.Severity, args.LogMessage.Internal.Source,
                args.LogMessage.Internal.Message, args.LogMessage.Internal.Exception);

        internal Task PrintVersion() => LogAsync(LogSeverity.Info, LogSource.Volte,
            $"Currently running Volte V{Version.FullVersion}.");

        public async Task LogAsync(LogSeverity s, LogSource src, string message, Exception e = null)
        {
            if (s is LogSeverity.Debug)
            {
                if (src is LogSource.Discord || src is LogSource.Gateway) { }

                if (src is LogSource.Volte && !Config.EnableDebugLogging) return;
            }

            await _semaphore.WaitAsync();
            await DoLogAsync(s, src, message, e);
            _ = _semaphore.Release();
        }

        private async Task DoLogAsync(LogSeverity s, LogSource src, string message, Exception e)
        {
            var (color, value) = VerifySeverity(s);
            await AppendAsync($"{value} -> ", color);

            (color, value) = VerifySource(src);
            await AppendAsync($"{value} -> ", color);

            if (!message.IsNullOrWhitespace())
                await AppendAsync(message, Color.White);

            if (e != null)
                await AppendAsync($"{e.Message}\n{e.StackTrace}", Color.IndianRed);

            await Console.Out.WriteAsync(Environment.NewLine);
        }

        private async Task AppendAsync(string m, Color c)
        {
            Console.ForegroundColor = c;
            await Console.Out.WriteAsync(m);
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