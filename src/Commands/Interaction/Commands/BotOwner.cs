using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Interaction.Commands
{
    public sealed class EvalCommand : ApplicationCommand
    {
        public EvalCommand() : base("Bot Owner Code Eval", ApplicationCommandType.Message) { }

        public override async Task HandleMessageCommandAsync(MessageCommandContext ctx)
        {
            await Task.Yield();
            Executor.Execute(async () =>
            {
                await EvalHelper.EvaluateAsync(ctx, ctx.UserMessage.Content);
            });
        }
    }
}