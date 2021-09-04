using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public sealed class EvalCommand : ApplicationCommand
    {
        public EvalCommand() : base("Code Eval", ApplicationCommandType.Message) { }

        public override Task<bool> RunMessageChecksAsync(MessageCommandContext ctx) 
            => Task.FromResult(ctx.GuildUser.IsBotOwner());

        public override async Task HandleMessageCommandAsync(MessageCommandContext ctx)
        {
            if (!await RunMessageChecksAsync(ctx))
                await ctx.CreateReplyBuilder(true).WithEmbed(x => x.WithTitle("You can't use this.").WithErrorColor()).RespondAsync();
            else
                Executor.Execute(async () => await EvalHelper.EvaluateAsync(ctx, ctx.UserMessage.Content));
        }
    }
}