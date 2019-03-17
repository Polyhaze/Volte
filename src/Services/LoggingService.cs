using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Volte.Data.Objects;
using Volte.Extensions;
using Console = Colorful.Console;
using Color = System.Drawing.Color;

namespace Volte.Services
{
    [Service("Logging", "The main Service used to handle logging to the bot's console.")]
    public sealed class LoggingService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        internal async Task Log(object obj, DebugLogMessageEventArgs args)
        {
            if (args.Message.Contains("handler is blocking")) return;
            var m = LogMessage.FromDiscordLogMessage(args);
            await Log(m.Level, m.Source, m.Message, m.Exception);
        }

        internal async Task PrintVersion()
        {
            await Log(LogLevel.Info, LogSource.Volte, $"Currently running Volte V{Version.FullVersion}");
        }

        public async Task Log(LogLevel s, LogSource src, string message, Exception e = null)
        {
            await _semaphore.WaitAsync();
            DoLog(s, src, message, e);
            _semaphore.Release();
        }

        private void DoLog(LogLevel s, LogSource src, string message, Exception e)
        {
            var (color, value) = VerifySeverity(s);
            Append($"{value} -> ", color);

            (color, value) = VerifySource(src);
            Append($"{value} -> ", color);

            if (!message.IsNullOrWhitespace())
                Append(message, Color.White);

            if (e != null)
                Append($"{e.Message}\n{e.StackTrace}", Color.IndianRed);


            Console.Write(Environment.NewLine);
        }

        private void Append(string m, Color c)
        {
            Console.ForegroundColor = c;
            Console.Write(m);
        }

        private (Color, string) VerifySource(LogSource source)
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
                case LogSource.DSharpPlus:
                    return (Color.Tomato, "REST");
                case LogSource.Unknown:
                    return (Color.Teal, "UNKN");
                default:
                    throw new ArgumentNullException(nameof(source), "source cannot be null.");
            }
        }

        private (Color, string) VerifySeverity(LogLevel s)
        {
            switch (s)
            {
                case LogLevel.Critical:
                    return (Color.Maroon, "CRIT");
                case LogLevel.Error:
                    return (Color.DarkRed, "EROR");
                case LogLevel.Warning:
                    return (Color.Yellow, "WARN");
                case LogLevel.Info:
                    return (Color.SpringGreen, "INFO");
                case LogLevel.Debug:
                    return (Color.SandyBrown, "DEBG");
                default:
                    throw new ArgumentNullException(nameof(s), "s cannot be null.");
            }
        }
    }
}