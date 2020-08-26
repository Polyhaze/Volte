using System;
using DSharpPlus;
using DSharpPlus.EventArgs;
using JetBrains.Annotations;

namespace Volte.Core.Models
{
    public sealed class LogMessage
    {
        public LogLevel Severity { get; private set; }
        public LogSource Source { get; private set; }
        public string Message { get; private set; }
        [CanBeNull]
        public Exception Exception { get; private set; }

        public static LogMessage FromDiscordLogMessage(DebugLogMessageEventArgs message)
            => new LogMessage
            {
                Message = message.Message,
                Severity = message.Level,
                Exception = message.Exception,
                Source = message.Application.ToLower() switch
                {
                    "rest" => LogSource.Rest,
                    "rest api" => LogSource.Rest,
                    "reset" => LogSource.Rest,
                    "discord" => LogSource.Discord,
                    "gateway" => LogSource.Gateway,
                    "websocket:dispatch" => LogSource.WebSocketDispatch,
                    "websocket" => LogSource.WebSocket,
                    "dsharpplus" => LogSource.DSharpPlus,
                    "interactivity" => LogSource.Interactivity,
                    "autoshard" => LogSource.AutoShard,
                    _ => LogSource.Unknown
                }
            };

        public static implicit operator LogMessage(DebugLogMessageEventArgs message) 
            => FromDiscordLogMessage(message);
    }
}