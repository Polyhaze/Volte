using System.Text;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Models.Misc;
using Volte.Services;

namespace Volte.Helpers
{
    public static class PollHelpers
    {
        public static PollInfo GetPollBody(string[] choices, EmojiService emojiService)
        {
            switch (choices.Length - 1)
            {
                case 1:
                {
                    return new PollInfo()
                        .AddFields(($"{new Emoji(emojiService.One)}", choices[1]));
                }

                case 2:
                {
                    return new PollInfo()
                        .AddFields(($"{new Emoji(emojiService.One)}", choices[1]),
                            ($"{new Emoji(emojiService.Two)}", choices[2]));
                }

                case 3:
                {
                    return new PollInfo()
                        .AddFields(($"{new Emoji(emojiService.One)}", choices[1]),
                            ($"{new Emoji(emojiService.Two)}", choices[2]),
                            ($"{new Emoji(emojiService.Three)}", choices[3]));
                }

                case 4:
                {
                    return new PollInfo()
                        .AddFields(($"{new Emoji(emojiService.One)}", choices[1]),
                            ($"{new Emoji(emojiService.Two)}", choices[2]),
                            ($"{new Emoji(emojiService.Three)}", choices[3]),
                            ($"{new Emoji(emojiService.Four)}", choices[4]));
                }

                case 5:
                    return new PollInfo()
                        .AddFields(($"{new Emoji(emojiService.One)}", choices[1]),
                            ($"{new Emoji(emojiService.Two)}", choices[2]),
                            ($"{new Emoji(emojiService.Three)}", choices[3]),
                            ($"{new Emoji(emojiService.Four)}", choices[4]),
                            ($"{new Emoji(emojiService.Five)}", choices[5]));

                default:
                {
                    return new PollInfo
                    {
                        IsValid = false
                    };
                }
            }
        }

        public static async Task AddPollReactionsAsync(string[] choices, IUserMessage msg, EmojiService EmojiService)
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