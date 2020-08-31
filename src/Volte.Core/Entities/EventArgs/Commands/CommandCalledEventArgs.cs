using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class CommandCalledEventArgs : CommandEventArgs
    {
        public override IResult Result { get; }
        public override VolteContext Context { get; }
        public override Stopwatch Stopwatch { get; }
        public override string FullMessageContent { get; }

        public CommandCalledEventArgs(IResult res, VolteContext context, Stopwatch sw)
        {
            Result = res;
            Context = context;
            Stopwatch = sw;
            FullMessageContent = context.Message.Content;
        }
    }
}
