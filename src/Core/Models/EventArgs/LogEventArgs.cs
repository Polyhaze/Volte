using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Volte.Core.Models.EventArgs
{
    public sealed class LogEventArgs : System.EventArgs
    {
        public string Message { get; }
        public string Source { get; }
        public LogLevel Severity { get; }
        public (LogMessage Internal, DebugLogMessageEventArgs Discord) LogMessage { get; }

        public LogEventArgs(DebugLogMessageEventArgs message)
        {
            Message = message.Message;
            Source = message.Application;
            Severity = message.Level;
            LogMessage = (message, message);
        }
    }
}