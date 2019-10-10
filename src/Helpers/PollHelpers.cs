using System.Text;
using System.Threading.Tasks;
using Discord;
using Volte.Services;

namespace Volte.Helpers
{
    public static class PollHelpers
    {
        public static string GetPollBody(string[] choices, EmojiService emojiService)
        {
            var sb = new StringBuilder();
            switch (choices.Length - 1)
            {
                case 1:
                {
                    return sb.AppendLine($"{new Emoji(emojiService.One)} {choices[1]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                }

                case 2:
                {
                    return sb.AppendLine($"{new Emoji(emojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(emojiService.Two)} {choices[2]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                }

                case 3:
                {
                    return sb.AppendLine($"{new Emoji(emojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(emojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[3]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                }

                case 4:
                {
                    return sb.AppendLine($"{new Emoji(emojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(emojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[3]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[4]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();
                }

                case 5:
                    return sb.AppendLine($"{new Emoji(emojiService.One)} {choices[1]}")
                        .AppendLine($"{new Emoji(emojiService.Two)} {choices[2]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[3]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[4]}")
                        .AppendLine($"{new Emoji(emojiService.Three)} {choices[5]}")
                        .AppendLine()
                        .AppendLine("Click the number below to vote.")
                        .ToString();

                default:
                {
                    return choices.Length - 1 > 5 ? "More than 5 options were specified." : "No options specified.";
                }
            }
        }

        public static async Task AddPollReactionsAsync(string[] choices, IUserMessage msg,EmojiService EmojiService)
        {
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
        }
    }
}
