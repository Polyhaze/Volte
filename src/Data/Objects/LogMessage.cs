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

            switch (args.Application.ToLower())
            {
                case "dsharpplus":
                    s.Source = LogSource.DSharpPlus;
                    return s;
                case "websocket":
                    s.Source = LogSource.WebSocket;
                    return s;
                case "websocket:dispatch":
                    s.Source = LogSource.WebSocketDispatch;
                    return s;
                case "event":
                    s.Source = LogSource.Event;
                    return s;
                case "autoshard":
                    s.Source = LogSource.AutoShard;
                    return s;
                case "voicenext":
                    s.Source = LogSource.VoiceNext;
                    return s;
                case "rest":
                    s.Source = LogSource.Rest;
                    return s;
                default:
                    s.Source = LogSource.Unknown;
                    return s;
            }
        }
    }
}