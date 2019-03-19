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

        public async Task Log(LogLevel severity, LogSource source, string message, Exception e = null)
        {
            await _semaphore.WaitAsync();
            DoLog(severity, source, message, e);
            _semaphore.Release();
        }

        private void DoLog(LogLevel s, LogSource src, string message, Exception e)
        {
            var (color, value) = GetSeverity(s);
            Append($"{value} -> ", color);

            (color, value) = GetSource(src);
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

        private (Color, string) GetSource(LogSource source)
        {
            switch (source)
            {
                case LogSource.Event:
                    return (Color.Orchid, "EVNT");
                case LogSource.Volte:
                    return (Color.Crimson, "CORE");
                case LogSource.WebSocket:
                case LogSource.WebSocketDispatch:
                    return (Color.RoyalBlue, "WSCK");
                case LogSource.AutoShard:
                    return (Color.Lime, "ASHD");
                case LogSource.Service:
                    return (Color.Gold, "SERV");
                case LogSource.VoiceNext:
                    return (Color.Indigo, "VNXT");
                case LogSource.Module:
                    return (Color.LimeGreen, "MDLE");
                case LogSource.DSharpPlus:
                    return (Color.Fuchsia, "");
                case LogSource.Rest:
                    return (Color.Teal, "REST");
                case LogSource.Unknown:
                    return (Color.Tomato, "UNKN");
                default:
                    throw new ArgumentNullException(nameof(source), "source cannot be null.");
            }
        }

        private (Color, string) GetSeverity(LogLevel s)
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