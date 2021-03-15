using Discord;
using System;

namespace Volte.Core.Entities
{
    public sealed class LogEventArgs : EventArgs
    {
        public string Message { get; }
        public string Source { get; }
        public LogSeverity Severity { get; }
        public (LogMessage Internal, Discord.LogMessage Discord) LogMessage { get; }

        public LogEventArgs(global::Discord.LogMessage message)
        {
            Message = message.Message;
            Source = message.Source;
            Severity = message.Severity;
            LogMessage = (Entities.LogMessage.FromDiscordLogMessage(message), message);
        }
    }
}