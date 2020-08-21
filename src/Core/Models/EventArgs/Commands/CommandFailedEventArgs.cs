using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Models.EventArgs
{
    public sealed class CommandFailedEventArgs : CommandEventArgs
    {
        public FailedResult FailedResult { get; }
        public override IResult Result { get; }
        public override VolteContext Context { get; }
        public override string FullMessageContent { get; }
        public override Stopwatch Stopwatch { get; }

        public CommandFailedEventArgs(FailedResult res, CommandEventArgs args)
        {
            Result = res;
            FailedResult = Result.Cast<FailedResult>();
            Context = args.Context;
            FullMessageContent = args.FullMessageContent;
            Stopwatch = args.Stopwatch;
        }
    }
}
