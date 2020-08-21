using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Interactivity.Enums;
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
                .WithTitle(Formatter.Bold(content[0]));

            foreach (var (key, value) in pollInfo.Fields)
            {
                embed.AddField(key, value, true);
            }

            return Ok(embed.WithFooter($"This poll will end in {duration.Humanize(3)}"), async msg =>
            {
                var result = await Context.Interactivity.DoPollAsync(msg, EmojiHelper.GetPollEmojisList().ToArray(), PollBehaviour.KeepEmojis, duration);
                embed = embed.WithTitle("Poll Ended! Here are the results:");
                foreach (var res in result)
                {
                    embed.AddField(res.Emoji.Name, res.Total);
                }

                await Context.ReplyAsync(embed);

            }, false);
        }
    }
}