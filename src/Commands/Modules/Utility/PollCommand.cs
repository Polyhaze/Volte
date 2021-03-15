using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("poll question;option1[;option2;option3;option4;option5]")]
        public Task<ActionResult> PollAsync([Remainder] string all)
        {
            var content = all.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var pollInfo = PollHelper.GetPollBody(content);
            if (!pollInfo.IsValid)
                return BadRequest(content.Length - 1 > 5
                    ? "More than 5 options were specified."
                    : "No options specified.");

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(Format.Bold(content[0]));
            
            return None(async () =>
            {
                var m = await PollHelper.ApplyPollInfo(embed, pollInfo).SendToAsync(Context.Channel);
                _ = await Context.Message.TryDeleteAsync("Poll invocation message.");
                await PollHelper.AddPollReactionsAsync(pollInfo.Fields.Count, m);
            });
        }
    }
}