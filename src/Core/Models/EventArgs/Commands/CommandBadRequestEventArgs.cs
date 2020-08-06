using System.Diagnostics;
using Volte.Commands;
using Volte.Commands.Results;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandBadRequestEventArgs : CommandEventArgs
    {
        public BadRequestResult Result { get; }
        public ResultCompletionData ResultCompletionData { get; }
        public override VolteContext Context { get; }
        public override string Arguments { get; }
        public override string Command { get; }
        public override Stopwatch Stopwatch { get; }

        public CommandBadRequestEventArgs(BadRequestResult res, ResultCompletionData data, CommandCalledEventArgs args)
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
