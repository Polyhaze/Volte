using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public sealed class EvalCommand : ApplicationCommand
    {
        public EvalCommand() : base("Bot Owner Code Eval", ApplicationCommandType.Message) { }

        public override Task HandleMessageCommandAsync(MessageCommandContext ctx)
        {
            Executor.Execute(async () => await EvalHelper.EvaluateAsync(ctx, ctx.UserMessage.Content));
            return Task.CompletedTask;
        }

    }
}