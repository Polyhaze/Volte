using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("Usage: |prefix|poll question;option1;option2;...")]
        public async Task Poll([Remainder] string pollText)
        {
            var question = pollText.Split(';')[0];
            var choices = pollText.Split(';');

            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder()
                .WithTitle(question)
                .WithAuthor(Context.User)
                .WithThumbnailUrl("http://survation.com/wp-content/uploads/2016/09/polleverywherelogo.png");
            string embedBody;

            switch (choices.Length - 1)
            {
                case 1:
                {
                    embedBody = $"{new Emoji(RawEmoji.ONE)} {choices[1]}\n\n" +
                                "Click the number below to vote.";
                    break;
                }
                case 2:
                {
                    embedBody = $"{new Emoji(RawEmoji.ONE)} {choices[1]}\n" +
                                $"{new Emoji(RawEmoji.TWO)} {choices[2]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 3:
                {
                    embedBody = $"{new Emoji(RawEmoji.ONE)} {choices[1]}\n" +
                                $"{new Emoji(RawEmoji.TWO)} {choices[2]}\n" +
                                $"{new Emoji(RawEmoji.THREE)} {choices[3]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 4:
                {
                    embedBody = $"{new Emoji(RawEmoji.ONE)} {choices[1]}\n" +
                                $"{new Emoji(RawEmoji.TWO)} {choices[2]}\n" +
                                $"{new Emoji(RawEmoji.THREE)} {choices[3]}\n" +
                                $"{new Emoji(RawEmoji.FOUR)} {choices[4]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 5:
                    embedBody = $"{new Emoji(RawEmoji.ONE)} {choices[1]}\n" +
                                $"{new Emoji(RawEmoji.TWO)} {choices[2]}\n" +
                                $"{new Emoji(RawEmoji.THREE)} {choices[3]}\n" +
                                $"{new Emoji(RawEmoji.FOUR)} {choices[4]}\n" +
                                $"{new Emoji(RawEmoji.FIVE)} {choices[5]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                default:
                {
                    if (choices.Length - 1 > 5)
                    {
                        embedBody = "More than 5 options were specified.";
                        break;
                    }

                    embedBody = "No options specified.";
                    break;
                }
            }

            embed.WithDescription(embedBody);

            var msg = await embed.SendTo(Context.Channel);
            await Context.Message.DeleteAsync();

            switch (choices.Length - 1)
            {
                case 1:
                {
                    await msg.AddReactionAsync(new Emoji(RawEmoji.ONE));
                    break;
                }
                case 2:
                {
                    await msg.AddReactionAsync(new Emoji(RawEmoji.ONE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.TWO));
                    break;
                }
                case 3:
                {
                    await msg.AddReactionAsync(new Emoji(RawEmoji.ONE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.TWO));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.THREE));
                    break;
                }
                case 4:
                {
                    await msg.AddReactionAsync(new Emoji(RawEmoji.ONE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.TWO));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.THREE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.FOUR));
                    break;
                }
                case 5:
                {
                    await msg.AddReactionAsync(new Emoji(RawEmoji.ONE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.TWO));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.THREE));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.FOUR));
                    await Task.Delay(500);
                    await msg.AddReactionAsync(new Emoji(RawEmoji.FIVE));
                    break;
                }
            }
        }
    }
}