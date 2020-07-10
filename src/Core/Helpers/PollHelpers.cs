using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;
using Volte.Core.Models.Misc;
using Volte.Services;

namespace Volte.Core.Helpers
{
    public static class PollHelpers
    {

        public static PollInfo GetPollBody(IEnumerable<string> choices, EmojiService e)
        {
            var c = choices as string[] ?? choices.ToArray();
            return PollInfo.FromDefaultFields(c.Length - 1, e, c);
        }

        public static async Task AddPollReactionsAsync(int amount, IUserMessage msg, EmojiService emojiService)
        {
            switch (amount)
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