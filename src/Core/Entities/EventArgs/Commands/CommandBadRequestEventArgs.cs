using System.Diagnostics;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class CommandBadRequestEventArgs : CommandEventArgs
    {
        public BadRequestResult Result { get; }
        public ResultCompletionData ResultCompletionData { get; }

        public CommandBadRequestEventArgs(BadRequestResult res, ResultCompletionData data, CommandEventArgs args)
        {
            Result = res;
            ResultCompletionData = data;
            Context = args.Context;
            Arguments = args.Arguments;
            Command = args.Command;
            Stopwatch = args.Stopwatch;
        }

        public string ExecutedLogMessage()
            => $"                    |           -Executed: {Result.IsSuccessful} | Reason: {Result.Reason}";
    }
}
