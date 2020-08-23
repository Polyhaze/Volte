using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Commands;
using Volte.Core.Models.Misc;

namespace Volte.Core.Helpers
{
    public static class PollHelpers
    {
        public static PollInfo GetPollBody(IEnumerable<string> choices)
        {
            var c = choices as string[] ?? choices.ToArray();
            return PollInfo.FromDefaultFields(c.Length - 1, c);
        }

        public static Task AddPollReactionsAsync(int amount, DiscordMessage msg)
        {
            var (one, two, three, four, five) = EmojiHelper.GetPollEmojis();

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
                await msg.CreateReactionAsync(one);
            }

            async Task Two()
            {
                await msg.CreateReactionAsync(one);
                await msg.CreateReactionAsync(two);
            }
            
            async Task Three()
            {
                await msg.CreateReactionAsync(one);
                await msg.CreateReactionAsync(two);
                await msg.CreateReactionAsync(three);
            }
            
            async Task Four()
            {
                await msg.CreateReactionAsync(one);
                await msg.CreateReactionAsync(two);
                await msg.CreateReactionAsync(three);
                await msg.CreateReactionAsync(four);
            }
            
            async Task Five()
            {
                await msg.CreateReactionAsync(one);
                await msg.CreateReactionAsync(two);
                await msg.CreateReactionAsync(three);
                await msg.CreateReactionAsync(four);
                await msg.CreateReactionAsync(five);
            }
            
        }
    }
}