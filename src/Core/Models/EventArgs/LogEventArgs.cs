using Discord;
using Discord.Commands;

namespace Volte.Core.Models.EventArgs
{
    public sealed class LogEventArgs : System.EventArgs
    {
        public LogEventArgs(Discord.LogMessage message)
        {
            Message = message.Message;
            Source = message.Source;
            Severity = message.Severity;
            LogMessage = (Models.LogMessage.FromDiscordLogMessage(message), message);
        }

        public string Message { get; }
        public string Source { get; }
        public LogSeverity Severity { get; }
        public (LogMessage Internal, Discord.LogMessage Discord) LogMessage { get; }
    }
}