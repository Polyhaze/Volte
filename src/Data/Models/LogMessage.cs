using System;
using Discord;

namespace Volte.Data.Models
{
    public sealed class LogMessage
    {
        public LogSeverity Severity { get; private set; }
        public LogSource Source { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public static LogMessage FromDiscordLogMessage(global::Discord.LogMessage message)
        {
            var s = new LogMessage
            {
                Message = message.Message,
                Severity = message.Severity,
                Exception = message.Exception
            };

            switch (message.Source)
            {
                case "Rest":
                    s.Source = LogSource.Rest;
                    return s;

                case "Discord":
                    s.Source = LogSource.Discord;
                    return s;

                case "Gateway":
                    s.Source = LogSource.Gateway;
                    return s;

                default:
                    s.Source = LogSource.Unknown;
                    return s;
            }
        }
    }
}