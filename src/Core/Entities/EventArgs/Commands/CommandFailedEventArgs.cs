using System.Diagnostics;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class CommandFailedEventArgs : CommandEventArgs
    {
        public FailedResult Result { get; }

        public CommandFailedEventArgs(FailedResult res, CommandEventArgs args)
        {
            Result = res;
            Context = args.Context;
            Arguments = args.Arguments;
            Command = args.Command;
            Stopwatch = args.Stopwatch;
        }

        public string ExecutedLogMessage(string reason)
            => $"                    |           -Executed: {Result.IsSuccessful} | Reason: {reason}";
    }
}
