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
                    "REST" => LogSource.Rest,
                    "REST API" => LogSource.Rest,
                    "RESET" => LogSource.Rest,
                    "Discord" => LogSource.Discord,
                    "Gateway" => LogSource.Gateway,
                    "WebSocket:Dispatch" => LogSource.WebSocketDispatch,
                    "WebSocket" => LogSource.WebSocket,
                    "DSharpPlus" => LogSource.DSharpPlus,
                    "Interactivity" => LogSource.Interactivity,
                    "Autoshard" => LogSource.AutoShard,
                    
                    _ => LogSource.Unknown
                }
            };

        public static implicit operator LogMessage(DebugLogMessageEventArgs message) 
            => FromDiscordLogMessage(message);
    }
}