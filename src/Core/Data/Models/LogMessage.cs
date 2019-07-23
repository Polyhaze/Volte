using System;
using Discord;

namespace Volte.Core.Data.Models
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

            var logSource = message.Source switch
                {
                "Rest" => LogSource.Rest,
                "Discord" => LogSource.Discord,
                "Gateway" => LogSource.Gateway,
                _ => LogSource.Unknown
                };
            s.Source = logSource;
            return s;
        }
    }
}