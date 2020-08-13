using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
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
        public Task<ActionResult> PollAsync(TimeSpan duration, [Remainder] string all)
        {
            var content = all.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var pollInfo = PollHelpers.GetPollBody(content);
            var choicesCount = content.Length - 1;
            if (!pollInfo.IsValid)
                return BadRequest(choicesCount > 5
                    ? "More than 5 options were specified."
                    : "No options specified.");

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(Format.Bold(content[0]));

            foreach (var (key, value) in pollInfo.Fields)
            {
                embed.AddField(key, value, true);
            }

            return Ok(embed.WithFooter($"This poll will end in {duration.Humanize(3)}"), async msg =>
            {
                
                _ = await Context.Message.TryDeleteAsync("Poll invocation message.");
                await PollHelpers.AddPollReactionsAsync(content.Length - 1, msg);
                await Executor.ExecuteAfterDelayAsync(duration, async () =>
                {
                    var result = Context.CreateEmbedBuilder().WithTitle("Poll Ended! Final Results:")
                        .WithDescription($"\"{content.First()}\"");
                    foreach (var (emoji, votes) in await PollHelpers.GetPollVotesAsync(Context, msg.Id, choicesCount))
                    {
                        var option = msg.Embeds.First().Fields.First(x => x.Name.Equals(emoji)).Value;
                        result.AddField(emoji, $"**{option}**: {votes}", true);
                    }

                    await msg.ModifyAsync(x => x.Embed = result.Build());
                });

            }, false);
        }
    }
}