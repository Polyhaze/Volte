using System;
using Discord;

namespace Volte.Core.Entities
{
    public sealed class LogMessage
    {
        public LogSeverity Severity { get; private init; }
        public LogSource Source { get; private init; }
        public string Message { get; private init; }
        public Exception Exception { get; private init; }

        public static LogMessage FromDiscordLogMessage(Discord.LogMessage message)
            => new()
            {
                Message = message.Message,
                Severity = message.Severity,
                Exception = message.Exception,
                Source = message.Source switch
                {
                    "Rest" => LogSource.Rest,
                    "Discord" => LogSource.Discord,
                    "Gateway" => LogSource.Gateway,
                    _ => LogSource.Unknown
                }
            };
    }
}