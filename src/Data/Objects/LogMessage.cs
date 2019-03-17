using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Volte.Data.Objects
{
    public sealed class LogMessage
    {
        public LogLevel Level { get; private set; }
        public LogSource Source { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public static LogMessage FromDiscordLogMessage(DebugLogMessageEventArgs args)
        {
            var s = new LogMessage
            {
                Message = args.Message,
                Level = args.Level,
                Exception = null
            };

            switch (args.Application)
            {
                case "DSharpPlus":
                    s.Source = LogSource.DSharpPlus;
                    return s;
                case "Websocket":
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