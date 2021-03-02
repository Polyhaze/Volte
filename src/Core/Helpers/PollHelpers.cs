using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Models;
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

        public static Task AddPollReactionsAsync(int amount, IUserMessage msg, EmojiService e)
        {
            var (one, two, three, four, five) = e.GetPollEmojis();

            return amount switch
            {
                1 => One(),
                2 => Two(),
                3 => Three(),
                4 => Four(),
                5 => Five(),
                _ => Task.CompletedTask
            };

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