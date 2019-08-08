using System.Text;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Poll")]
        [Description("Create a poll.")]
        [Remarks("Usage: |prefix|poll question;option1;option2;...")]
        public Task<ActionResult> PollAsync([Remainder] string pollText)
        {
            var question = pollText.Split(';')[0];
            var choices = pollText.Split(';');

            string embedBody;
            var sb = new StringBuilder();

            switch (choices.Length - 1)
            {
                case 1:
                {
                    embedBody = sb.AppendLine($"{new Emoji(EmojiService.One)} {choices[1]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                    break;
                }

                case 2:
                {
                    embedBody = sb.AppendLine($"{new Emoji(EmojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(EmojiService.Two)} {choices[2]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                    break;
                }

                case 3:
                {
                    embedBody = sb.AppendLine($"{new Emoji(EmojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(EmojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[3]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                    break;
                }

                case 4:
                {
                    embedBody = sb.AppendLine($"{new Emoji(EmojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(EmojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[3]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[4]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                    break;
                }

                case 5:
                    embedBody = sb.AppendLine($"{new Emoji(EmojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(EmojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[3]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[4]}")
                        .AppendLine($"{new Emoji(EmojiService.Three)} {choices[5]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
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