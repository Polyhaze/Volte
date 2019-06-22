using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Services
{
    [Service("Logging", "The main Service used to handle logging to the bot's console.")]
    public sealed class LoggingService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        internal async Task Log(LogEventArgs args)
        {
            await LogAsync(args.LogMessage.Internal.Severity, args.LogMessage.Internal.Source,
                args.LogMessage.Internal.Message, args.LogMessage.Internal.Exception);
        }

        internal Task PrintVersion() => LogAsync(LogSeverity.Info, LogSource.Volte,
            $"Currently running Volte V{Version.FullVersion}.");

        public async Task LogAsync(LogSeverity s, LogSource src, string message, Exception e = null)
        {
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

            Console.Write(Environment.NewLine);
        }

        private async Task AppendAsync(string m, Color c)
        {
            Console.ForegroundColor = c;
            await Console.Out.WriteAsync(m);
        }

        private (Color Color, string Source) VerifySource(LogSource source)
        {
            switch (source)
            {
                case LogSource.Discord:
                case LogSource.Gateway:
                    return (Color.RoyalBlue, "DSCD");

                case LogSource.Volte:
                    return (Color.Crimson, "CORE");

                case LogSource.Service:
                    return (Color.Gold, "SERV");

                case LogSource.Module:
                    return (Color.LimeGreen, "MDLE");

                case LogSource.Rest:
                    return (Color.Tomato, "REST");

                case LogSource.Unknown:
                    return (Color.Teal, "UNKN");

                default:
                    throw new ArgumentNullException(nameof(source), "source cannot be null.");
            }
        }

        private (Color Color, string Level) VerifySeverity(LogSeverity s)
        {
            switch (s)
            {
                case LogSeverity.Critical:
                    return (Color.Maroon, "CRIT");

                case LogSeverity.Error:
                    return (Color.DarkRed, "EROR");

                case LogSeverity.Warning:
                    return (Color.Yellow, "WARN");

                case LogSeverity.Info:
                    return (Color.SpringGreen, "INFO");

                case LogSeverity.Verbose:
                    return (Color.Pink, "VRBS");

                case LogSeverity.Debug:
                    return (Color.SandyBrown, "DEBG");

                default:
                    throw new ArgumentNullException(nameof(s), "s cannot be null.");
            }
        }
    }
}