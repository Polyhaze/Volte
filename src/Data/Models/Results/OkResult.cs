using System;
using System.Threading.Tasks;
using Discord;
using Volte.Commands;
using Volte.Extensions;

namespace Volte.Data.Models.Results
{
    public class OkResult : BaseResult
    {
        public OkResult(string text, bool shouldEmbed = true, Func<IUserMessage, Task> func = null)
        {
            Message = text;
            ShouldEmbed = shouldEmbed;
            After = func;
        }

        public override bool IsSuccessful => true;

        private string Message { get; }
        private bool ShouldEmbed { get; }
        private Func<IUserMessage, Task> After { get; }

        public override async Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var currentUser = await ctx.Guild.GetCurrentUserAsync();
            if (!currentUser.GetPermissions(ctx.Channel).SendMessages) return new ResultCompletionData();

            IUserMessage message;
            if (ShouldEmbed)
            {
                message = await ctx.CreateEmbed(Message).SendToAsync(ctx.Channel);
            }
            else
            {
                message = await ctx.Channel.SendMessageAsync(Message);
            }

            if (After != null)
            {
                await After(message);
            }


            return new ResultCompletionData();
        }
    }
}