using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("poll question;option1[;option2;option3;option4;option5]")]
        public Task<ActionResult> PollAsync([Remainder] string pollText)
        {
            var choices = pollText.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var pollInfo = PollHelpers.GetPollBody(choices, EmojiService);
            if (!pollInfo.IsValid)
                return BadRequest(choices.Length - 1 > 5
                    ? "More than 5 options were specified."
                    : "No options specified.");

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(Format.Bold(choices[0]));

            foreach (var (name, value) in pollInfo.Fields)
            {
                embed.AddField(name, value, true);
            }

            return Ok(embed.WithFooter(pollInfo.Footer), async msg =>
            {
                _ = await Context.Message.TryDeleteAsync();
                await PollHelpers.AddPollReactionsAsync(choices, msg, EmojiService);
            });
        }
    }
}