using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandCalledEventArgs : CommandEventArgs
    {
        public IResult Result { get; }
        public override VolteContext Context { get; }
        public Stopwatch Stopwatch { get; }

        public CommandCalledEventArgs(IResult res, CommandContext context, Stopwatch sw)
        {
            Result = res;
            Context = context.Cast<VolteContext>();
            Stopwatch = sw;
        }
    }
}
