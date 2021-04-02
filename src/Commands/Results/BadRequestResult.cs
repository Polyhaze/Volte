using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;

namespace Volte.Commands
{
    public class BadRequestResult : ActionResult
    {
        public BadRequestResult(string reason) 
            => Reason = reason;

        public string Reason { get; }

        public override bool IsSuccessful => false;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var shouldReply = ctx.GuildData.Configuration.ReplyInline;
            var e = ctx.CreateEmbedBuilder()
                .WithTitle("No can do, partner.")
                .WithDescription(Reason)
                .WithCurrentTimestamp();
            IUserMessage message;
            if (shouldReply)
                message = await e.ReplyToAsync(ctx.Message);
            else
                message = await e.SendToAsync(ctx.Channel);
            return new ResultCompletionData(message);
        }
    }
}