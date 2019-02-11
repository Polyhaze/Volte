using System;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using static Colorful.Console;
using Discord;
using Volte.Core.Data.Objects;
using LogMessage = Discord.LogMessage;
using Version = Volte.Core.Runtime.Version;

namespace Volte.Core.Services {
    public class LoggingService {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        internal async Task Log(LogMessage msg) {
            var m = Data.Objects.LogMessage.FromDiscordLogMessage(msg);
            await Log(m.Severity, m.Source, m.Message, m.Exception);
        }

        internal async Task PrintVersion() {
            await Log(LogSeverity.Info, LogSource.Volte, $"Currently running Volte V{Version.GetFullVersion()}");
        }

        public async Task Log(LogSeverity s, LogSource src, string message, Exception e = null) {
            await _semaphore.WaitAsync();
            DoLog(s, src, message, e);
            _semaphore.Release();
        }

        private void DoLog(LogSeverity s, LogSource src, string message, Exception e) {
            var (color, value) = VerifySeverity(s);
            Append($"{value} -> ", color);

            (color, value) = VerifySource(src);
            Append($"{value} -> ", color);

            if (!string.IsNullOrWhiteSpace(message))
                Append(message, Color.White);

            if (e != null)
                Append($"{e.Message}\n{e.StackTrace}", Color.IndianRed);


            Write(Environment.NewLine);
        }

        private void Append(string m, Color c) {
            ForegroundColor = c;
            Write(m);
        }

        private (Color, string) VerifySource(LogSource source) {
            switch (source) {
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

        private (Color, string) VerifySeverity(LogSeverity s) {
            switch (s) {
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