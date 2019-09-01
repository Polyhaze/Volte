using System.Diagnostics;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandFailedEventArgs : CommandEventArgs
    {
        public FailedResult FailedResult { get; }
        public override VolteContext Context { get; }
        public string Arguments { get; }
        public Stopwatch Stopwatch { get; }

        public CommandFailedEventArgs(FailedResult res, VolteContext ctx, string args,
            Stopwatch sw)
        {
            FailedResult = res;
            Context = ctx;
            Arguments = args;
            Stopwatch = sw;
        }
    }
}
