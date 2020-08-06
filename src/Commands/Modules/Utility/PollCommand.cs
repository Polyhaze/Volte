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
            var pollInfo = PollHelpers.GetPollBody(content, EmojiService);
            if (!pollInfo.IsValid)
                return BadRequest((content.Length - 1) > 5
                    ? "More than 5 options were specified."
                    : "No options specified.");

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(Format.Bold(content[0]));

            foreach (var (key, value) in pollInfo.Fields)
            {
                embed.AddField(key, value, true);
            }

            return Ok(embed.WithFooter(pollInfo.Footer), async msg =>
            {
                _ = await Context.Message.TryDeleteAsync("Poll invocation message.");
                await PollHelpers.AddPollReactionsAsync(content.Length - 1, msg, EmojiService);
            });
        }
    }
}