using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("Usage: |prefix|poll question;option1;option2;...")]
        public async Task PollAsync([Remainder] string pollText)
        {
            var question = pollText.Split(';')[0];
            var choices = pollText.Split(';');

            var embed = Context.CreateEmbedBuilder()
                .WithTitle(question)
                .WithThumbnailUrl("http://survation.com/wp-content/uploads/2016/09/polleverywherelogo.png");
            string embedBody;

            switch (choices.Length - 1)
            {
                case 1:
                {
                    embedBody = $"{DiscordEmoji.FromUnicode(EmojiService.ONE)} {choices[1]}\n\n" +
                                "Click the number below to vote.";
                    break;
                }
                case 2:
                {
                    embedBody = $"{DiscordEmoji.FromUnicode(EmojiService.ONE)} {choices[1]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.TWO)} {choices[2]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 3:
                {
                    embedBody = $"{DiscordEmoji.FromUnicode(EmojiService.ONE)} {choices[1]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.TWO)} {choices[2]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.THREE)} {choices[3]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 4:
                {
                    embedBody = $"{DiscordEmoji.FromUnicode(EmojiService.ONE)} {choices[1]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.TWO)} {choices[2]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.THREE)} {choices[3]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.FOUR)} {choices[4]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }
                case 5:
                    embedBody = $"{DiscordEmoji.FromUnicode(EmojiService.ONE)} {choices[1]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.TWO)} {choices[2]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.THREE)} {choices[3]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.FOUR)} {choices[4]}\n" +
                                $"{DiscordEmoji.FromUnicode(EmojiService.FIVE)} {choices[5]}\n\n" +
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

            var msg = await embed.SendToAsync(Context.Channel);
            await Context.Message.DeleteAsync();

            switch (choices.Length - 1)
            {
                case 1:
                {
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.ONE));
                    break;
                }
                case 2:
                {
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.ONE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.TWO));
                    break;
                }
                case 3:
                {
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.ONE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.TWO));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.THREE));
                    break;
                }
                case 4:
                {
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.ONE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.TWO));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.THREE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.FOUR));
                    break;
                }
                case 5:
                {
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.ONE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.TWO));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.THREE));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.FOUR));
                    await msg.CreateReactionAsync(DiscordEmoji.FromUnicode(EmojiService.FIVE));
                    break;
                }
            }
        }
    }
}