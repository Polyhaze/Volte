using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class CommandCalledEventArgs : CommandEventArgs
    {
        public IResult Result { get; }
        public override VolteContext Context { get; }
        public override Stopwatch Stopwatch { get; }
        public override string Command { get; }
        public override string Arguments { get; }

        public CommandCalledEventArgs(IResult res, CommandContext context, Stopwatch sw)
        {
            Result = res;
            Context = context.Cast<VolteContext>();
            Stopwatch = sw;
            Command = Context.Message.Content.Split(" ")[0];
            Arguments = Context.Message.Content.Replace($"{Command}", "").Trim();
            if (string.IsNullOrEmpty(Arguments)) Arguments = "None";
        }

        public string ExecutedLogMessage()
            => $"                    |           -Executed: {Result.IsSuccessful}";
        }
}
