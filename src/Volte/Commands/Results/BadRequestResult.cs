using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;

namespace Volte.Commands.Results
{
    public class BadRequestResult : ActionResult
    {
        public BadRequestResult(string reason) 
            => Reason = reason;

        public string Reason { get; }

        public override bool IsSuccessful => false;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var message = await ctx.CreateEmbedBuilder()
                .WithTitle("No can do, partner.")
                .WithDescription(Reason)
                .WithColor(new Color(Config.ErrorColor))
                .WithTimestamp(DateTimeOffset.Now)
                .SendToAsync(ctx.Channel);
            return new ResultCompletionData(message);
        }
    }
}