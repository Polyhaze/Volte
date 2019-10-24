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
        public string Arguments { get; }
        public Stopwatch Stopwatch { get; }

        public CommandBadRequestEventArgs(BadRequestResult res, ResultCompletionData data, VolteContext ctx, string args, Stopwatch sw)
        {
            Result = res;
            ResultCompletionData = data;
            Context = ctx;
            Arguments = args;
            Stopwatch = sw;
        }
    }
}
