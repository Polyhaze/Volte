using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Volte.Core.Models
{
    public sealed class LogMessage
    {
        public LogLevel Severity { get; private set; }
        public LogSource Source { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public static LogMessage FromDiscordLogMessage(DebugLogMessageEventArgs message)
            => new LogMessage
            {
                Message = message.Message,
                Severity = message.Level,
                Exception = message.Exception,
                Source = message.Application switch
                {
                    "Rest" => LogSource.Rest,
                    "Discord" => LogSource.Discord,
                    "Gateway" => LogSource.Gateway,
                    _ => LogSource.Unknown
                }
            };

        public static implicit operator LogMessage(DebugLogMessageEventArgs message) 
            => FromDiscordLogMessage(message);
    }
}