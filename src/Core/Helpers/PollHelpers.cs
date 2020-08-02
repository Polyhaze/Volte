using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
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

        public static async Task AddPollReactionsAsync(int amount, IUserMessage msg, EmojiService e)
        {
            var (one, two, three, four, five) = e.GetPollEmojis();

            switch (amount)
            {
                case 1:
                    await One();
                    break;

                case 2:
                    await Two();
                    break;

                case 3:
                    await Three();
                    break;

                case 4:
                    await Four();
                    break;

                case 5:
                    await Five();
                    break;
            }

            async Task One()
            {
                await msg.AddReactionAsync(one);
            }

            async Task Two()
            {
                await msg.AddReactionAsync(one);
                await msg.AddReactionAsync(two);
            }
            
            async Task Three()
            {
                await msg.AddReactionAsync(one);
                await msg.AddReactionAsync(two);
                await msg.AddReactionAsync(three);
            }
            
            async Task Four()
            {
                await msg.AddReactionAsync(one);
                await msg.AddReactionAsync(two);
                await msg.AddReactionAsync(three);
                await msg.AddReactionAsync(four);
            }
            
            async Task Five()
            {
                await msg.AddReactionAsync(one);
                await msg.AddReactionAsync(two);
                await msg.AddReactionAsync(three);
                await msg.AddReactionAsync(four);
                await msg.AddReactionAsync(five);
            }
            
        }
    }
}