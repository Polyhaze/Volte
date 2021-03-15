using System.Diagnostics;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class CommandCalledEventArgs : CommandEventArgs
    {
        public IResult Result { get; }

        public CommandCalledEventArgs(IResult res, CommandContext context, Stopwatch sw)
        {
            Result = res;
            Context = context.Cast<VolteContext>();
            Stopwatch = sw;
            Command = Context.Message.Content.Split(" ")[0];
            Arguments = Context.Message.Content.Replace($"{Command}", "").Trim();
            if (Arguments.IsNullOrEmpty()) Arguments = "None";
        }

        public string ExecutedLogMessage()
            => $"                    |           -Executed: {Result.IsSuccessful}";
        }
}
