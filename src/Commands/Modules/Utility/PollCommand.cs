using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
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
                    embedBody = $"{new Emoji(EmojiService.One)} {choices[1]}\n\n" +
                                "Click the number below to vote.";
                    break;
                }

                case 2:
                {
                    embedBody = $"{new Emoji(EmojiService.One)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.Two)} {choices[2]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 3:
                {
                    embedBody = $"{new Emoji(EmojiService.One)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.Two)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.Three)} {choices[3]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 4:
                {
                    embedBody = $"{new Emoji(EmojiService.One)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.Two)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.Three)} {choices[3]}\n" +
                                $"{new Emoji(EmojiService.Four)} {choices[4]}\n\n" +
                                "Click one of the numbers below to vote.";
                    break;
                }

                case 5:
                    embedBody = $"{new Emoji(EmojiService.One)} {choices[1]}\n" +
                                $"{new Emoji(EmojiService.Two)} {choices[2]}\n" +
                                $"{new Emoji(EmojiService.Three)} {choices[3]}\n" +
                                $"{new Emoji(EmojiService.Four)} {choices[4]}\n" +
                                $"{new Emoji(EmojiService.Five)} {choices[5]}\n\n" +
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
                            await msg.AddReactionAsync(new Emoji(EmojiService.One));
                            break;
                        }

                        case 2:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.One));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Two));
                            break;
                        }

                        case 3:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.One));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Two));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Three));
                            break;
                        }

                        case 4:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.One));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Two));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Three));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Four));
                            break;
                        }

                        case 5:
                        {
                            await msg.AddReactionAsync(new Emoji(EmojiService.One));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Two));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Three));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Four));
                            await msg.AddReactionAsync(new Emoji(EmojiService.Five));
                            break;
                        }
                    }
                });
        }
    }
}