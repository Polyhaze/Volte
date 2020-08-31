using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Commands.Results;

namespace Volte.Core.Entities
{
    public sealed class CommandBadRequestEventArgs : CommandEventArgs
    {
        public BadRequestResult BadResult { get; }
        public override IResult Result { get; }
        public ResultCompletionData ResultCompletionData { get; }
        public override VolteContext Context { get; }
        public override string FullMessageContent { get; }
        public override Stopwatch Stopwatch { get; }

        public CommandBadRequestEventArgs(BadRequestResult res, ResultCompletionData data, CommandEventArgs args)
        {
            Result = res;
            BadResult = Result.Cast<BadRequestResult>();
            ResultCompletionData = data;
            Context = args.Context;
            FullMessageContent = Context.Message.Content;
            Stopwatch = args.Stopwatch;
        }
    }
}
