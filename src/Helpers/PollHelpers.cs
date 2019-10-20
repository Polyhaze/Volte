using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core.Models.Misc;
using Volte.Services;

namespace Volte.Helpers
{
    public static class PollHelpers
    {
        public static PollInfo GetPollBody(string[] choices, EmojiService emojiService)
            => (choices.Length - 1) switch
            {
                1 => PollInfo.FromFields(($"{emojiService.One.ToEmoji()}", choices[1])),

                2 => PollInfo.FromFields(($"{emojiService.One.ToEmoji()}", choices[1]),
                        ($"{emojiService.Two.ToEmoji()}", choices[2])),

                3 => PollInfo.FromFields(($"{emojiService.One.ToEmoji()}", choices[1]),
                        ($"{emojiService.Two.ToEmoji()}", choices[2]),
                        ($"{emojiService.Three.ToEmoji()}", choices[3])),

                4 => PollInfo.FromFields(($"{emojiService.One.ToEmoji()}", choices[1]),
                        ($"{emojiService.Two.ToEmoji()}", choices[2]),
                        ($"{emojiService.Three.ToEmoji()}", choices[3]),
                        ($"{emojiService.Four.ToEmoji()}", choices[4])),

                5 => PollInfo.FromFields(($"{emojiService.One.ToEmoji()}", choices[1]),
                        ($"{emojiService.Two.ToEmoji()}", choices[2]),
                        ($"{emojiService.Three.ToEmoji()}", choices[3]),
                        ($"{emojiService.Four.ToEmoji()}", choices[4]),
                        ($"{emojiService.Five.ToEmoji()}", choices[5])),

                _ => PollInfo.FromValid(false)
            };

        public static async Task AddPollReactionsAsync(string[] choices, IUserMessage msg, EmojiService emojiService)
        {
            switch (choices.Length - 1)
            {
                case 1:
                    await msg.AddReactionAsync(emojiService.One.ToEmoji());
                    break;

                case 2:
                    await msg.AddReactionAsync(emojiService.One.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Two.ToEmoji());
                    break;

                case 3:
                    await msg.AddReactionAsync(emojiService.One.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Two.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Three.ToEmoji());
                    break;

                case 4:
                    await msg.AddReactionAsync(emojiService.One.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Two.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Three.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Four.ToEmoji());
                    break;

                case 5:
                    await msg.AddReactionAsync(emojiService.One.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Two.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Three.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Four.ToEmoji());
                    await msg.AddReactionAsync(emojiService.Five.ToEmoji());
                    break;
            }
        }
    }
}