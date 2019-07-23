using System;
using System.Threading.Tasks;
using Discord;
using Volte.Commands;
using Gommon;

namespace Volte.Core.Data.Models.Results
{
    public class BadRequestResult : VolteCommandResult
    {
        public BadRequestResult(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }

        public override bool IsSuccessful => false;

        public override async Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
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