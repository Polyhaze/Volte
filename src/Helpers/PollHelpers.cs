using System.Threading.Tasks;
using Discord;
using Volte.Core.Models.Misc;
using Volte.Services;

namespace Volte.Helpers
{
    public static class PollHelpers
    {
        public static PollInfo GetPollBody(string[] choices, EmojiService emojiService)
            => (choices.Length - 1) switch
            {
                1 => PollInfo.FromFields(($"{new Emoji(emojiService.One)}", choices[1])),

                2 => PollInfo.FromFields(($"{new Emoji(emojiService.One)}", choices[1]),
                        ($"{new Emoji(emojiService.Two)}", choices[2])),

                3 => PollInfo.FromFields(($"{new Emoji(emojiService.One)}", choices[1]),
                        ($"{new Emoji(emojiService.Two)}", choices[2]),
                        ($"{new Emoji(emojiService.Three)}", choices[3])),

                4 => PollInfo.FromFields(($"{new Emoji(emojiService.One)}", choices[1]),
                        ($"{new Emoji(emojiService.Two)}", choices[2]),
                        ($"{new Emoji(emojiService.Three)}", choices[3]),
                        ($"{new Emoji(emojiService.Four)}", choices[4])),

                5 => PollInfo.FromFields(($"{new Emoji(emojiService.One)}", choices[1]),
                        ($"{new Emoji(emojiService.Two)}", choices[2]),
                        ($"{new Emoji(emojiService.Three)}", choices[3]),
                        ($"{new Emoji(emojiService.Four)}", choices[4]),
                        ($"{new Emoji(emojiService.Five)}", choices[5])),

                _ => PollInfo.FromValid(false)
            };

        public static async Task AddPollReactionsAsync(string[] choices, IUserMessage msg, EmojiService emojiService)
        {
            switch (choices.Length - 1)
            {
                case 1:
                    await msg.AddReactionAsync(new Emoji(emojiService.One));
                    break;

                case 2:
                    await msg.AddReactionAsync(new Emoji(emojiService.One));
                    await msg.AddReactionAsync(new Emoji(emojiService.Two));
                    break;

                case 3:
                    await msg.AddReactionAsync(new Emoji(emojiService.One));
                    await msg.AddReactionAsync(new Emoji(emojiService.Two));
                    await msg.AddReactionAsync(new Emoji(emojiService.Three));
                    break;

                case 4:
                    await msg.AddReactionAsync(new Emoji(emojiService.One));
                    await msg.AddReactionAsync(new Emoji(emojiService.Two));
                    await msg.AddReactionAsync(new Emoji(emojiService.Three));
                    await msg.AddReactionAsync(new Emoji(emojiService.Four));
                    break;

                case 5:
                    await msg.AddReactionAsync(new Emoji(emojiService.One));
                    await msg.AddReactionAsync(new Emoji(emojiService.Two));
                    await msg.AddReactionAsync(new Emoji(emojiService.Three));
                    await msg.AddReactionAsync(new Emoji(emojiService.Four));
                    await msg.AddReactionAsync(new Emoji(emojiService.Five));
                    break;
            }
        }
    }
}