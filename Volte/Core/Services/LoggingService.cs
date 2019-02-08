using System;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using static Colorful.Console;
using Discord;
using Version = Volte.Core.Runtime.Version;

namespace Volte.Core.Services {
    public class LoggingService {

        private readonly object _lock = new object();

        internal Task Log(LogMessage m) {
            Log(m.Severity, m.Source, m.Message, m.Exception);
            return Task.CompletedTask;
        }

        internal void PrintVersion() {
            Log(LogSeverity.Info, "Volte", $"Currently running Volte V{Version.GetFullVersion()}");
        }

        public void Log(LogSeverity s, string src, string message, Exception e = null) {
            lock (_lock) {
                DoLog(s, src, message, e);   
            }
        }

        public void DoLog(LogSeverity s, string src, string message, Exception e) {

            var (color, value) = VerifySeverity(s);
            Append($"{value} -> ", color);

            (color, value) = VerifySource(src);
            Append($"{value} -> ", color);

            if (!string.IsNullOrWhiteSpace(message))
                Append(message, Color.White);

            if (e != null)
                Append(e.Message, Color.IndianRed);

            Write(Environment.NewLine);
        }

        private void Append(string m, Color c) {
            ForegroundColor = c;
            Write(m);
        }

        private (Color, string) VerifySource(string source) {
            switch (source) {
                case "Discord":
                case "Gateway":
                    return (Color.RoyalBlue, "DSCD");
                case "Volte":
                    return (Color.Crimson, "CORE");
                case "Service":
                    return (Color.Gold, "SERV");
                case "Module":
                    return (Color.LimeGreen, "MDLE");
                case "Rest":
                    return (Color.Tomato, "REST");
                default:
                    return (Color.Teal, "UNKN");
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
