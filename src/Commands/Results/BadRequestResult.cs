using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte;
using Volte.Helpers;

namespace Volte.Commands
{
    public class BadRequestResult : ActionResult
    {
        public BadRequestResult(string reason)
            => Reason = reason;

        public BadRequestResult(Embed embed)
            => Embed = embed;

        public string Reason { get; }
        public Embed Embed { get; }

        public override bool IsSuccessful => false;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var e = Embed ?? ctx.CreateEmbedBuilder()
                .WithTitle("No can do, partner.")
                .WithDescription(Reason)
                .WithCurrentTimestamp()
                .Build();

            return new ResultCompletionData(ctx.GuildData.Configuration.ReplyInline
                ? await e.ReplyToAsync(ctx.Message)
                : await e.SendToAsync(ctx.Channel));
        }
    }
}