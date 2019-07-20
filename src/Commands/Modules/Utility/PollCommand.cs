using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("Usage: |prefix|poll question;option1;option2;...")]
        public Task<VolteCommandResult> PollAsync([Remainder] string pollText)
        {
            var question = pollText.Split(';')[0];
            var choices = pollText.Split(';');

            string embedBody;

            switch (choices.Length - 1)
            {
                case 1:
                {
                    embedBody = $"{new Emoji(EmojiService.ONE)} {choices[1]}\n\n" +
                                "Click the number below to vote.";
                    break;
                }

                case 2:
                {
                    embedBody = $"{new Emoji(EmojiService.ONE)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.TWO)} {choices[2]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 3:
                {
                    embedBody = $"{new Emoji(EmojiService.ONE)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.TWO)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.THREE)} {choices[3]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 4:
                {
                    embedBody = $"{new Emoji(EmojiService.ONE)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.TWO)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.THREE)} {choices[3]}\n" +
                                $"{new Emoji(EmojiService.FOUR)} {choices[4]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 5:
                    embedBody = $"{new Emoji(EmojiService.ONE)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.TWO)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.THREE)} {choices[3]}\n" +
                                $"{new Emoji(EmojiService.FOUR)} {choices[4]}\n" +
                                $"{new Emoji(EmojiService.FIVE)} {choices[5]}\n\n" +
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


            return Ok(Context.CreateEmbedBuilder()
                    .WithTitle(question)
                    .WithThumbnailUrl("http://survation.com/wp-content/uploads/2016/09/polleverywherelogo.png")
                    .WithDescription(embedBody),
                async msg =>
                {
                    await Context.Message.DeleteAsync();
                    switch (choices.Length - 1)
                    {
                        case 1:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.ONE));
                            break;
                        }

                        case 2:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.ONE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.TWO));
                            break;
                        }

                        case 3:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.ONE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.TWO));
                            await msg.AddReactionAsync(new Emoji(EmojiService.THREE));
                            break;
                        }

                        case 4:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.ONE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.TWO));
                            await msg.AddReactionAsync(new Emoji(EmojiService.THREE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.FOUR));
                            break;
                        }

                        case 5:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.ONE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.TWO));
                            await msg.AddReactionAsync(new Emoji(EmojiService.THREE));
                            await msg.AddReactionAsync(new Emoji(EmojiService.FOUR));
                            await msg.AddReactionAsync(new Emoji(EmojiService.FIVE));
                            break;
                        }
                    }
                });
        }
    }
}