using System.Diagnostics;
using Volte.Commands;
using Volte.Commands.Results;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandBadRequestEventArgs : CommandEventArgs
    {
        public BadRequestResult BadRequestResult { get; }
        public override VolteContext Context { get; }
        public string Arguments { get; }
        public Stopwatch Stopwatch { get; }

        public CommandBadRequestEventArgs(BadRequestResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            BadRequestResult = res;
            Context = ctx;
            Arguments = args;
            Stopwatch = sw;
        }
    }
}
