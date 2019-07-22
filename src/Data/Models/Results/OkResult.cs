using System;
using System.Threading.Tasks;
using Discord;
using Volte.Commands;
using Gommon;

namespace Volte.Data.Models.Results
{
    public class OkResult : VolteCommandResult
    {
        public OkResult(string text, bool shouldEmbed = true, EmbedBuilder embed = null,
            Func<IUserMessage, Task> func = null)
        {
            Message = text;
            ShouldEmbed = shouldEmbed;
            Embed = embed;
            After = func;
        }

        public OkResult(Func<Task> logic)
        {
            SeparateLogic = logic;
        }

        private string Message { get; }
        private bool ShouldEmbed { get; }
        private Func<IUserMessage, Task> After { get; }
        private Func<Task> SeparateLogic { get; }
        private EmbedBuilder Embed { get; }

        public override async Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var currentUser = await ctx.Guild.GetCurrentUserAsync();
            if (!currentUser.GetPermissions(ctx.Channel).SendMessages) return new ResultCompletionData();

            if (SeparateLogic != null)
            {
                await SeparateLogic();
                return new ResultCompletionData();
            }

            IUserMessage message;
            if (Embed is null)
            {
                if (ShouldEmbed)
                {
                    message = await ctx.CreateEmbed(Message).SendToAsync(ctx.Channel);
                }
                else
                {
                    message = await ctx.Channel.SendMessageAsync(Message);
                }
            }
            else
            {
                message = await Embed.SendToAsync(ctx.Channel);
            }


            if (After != null)
            {
                await After(message);
            }


            return new ResultCompletionData(message);
        }
    }
}