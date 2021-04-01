using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Core.Entities;

namespace Volte.Core.Helpers
{
    public static class PollHelper
    {
        public static PollInfo GetPollBody(IEnumerable<string> choices) 
            => PollInfo.FromDefaultFields(choices as string[] ?? choices.ToArray());
        
        public static EmbedBuilder ApplyPollInfo(EmbedBuilder embedBuilder, PollInfo pollInfo)
        {
            foreach (var (key, value) in pollInfo.Fields)
                embedBuilder.AddField(key, value, true);

            embedBuilder.WithFooter(PollInfo.Footer);

            return embedBuilder;
        }

        public static Task AddPollReactionsAsync(int amount, IUserMessage msg)
        {
            var (one, two, three, four, five) = DiscordHelper.GetPollEmojis();

            return amount switch
            {
                1 => One(),
                2 => Two(),
                3 => Three(),
                4 => Four(),
                5 => Five(),
                _ => Task.CompletedTask
            };

            Task One() => msg.AddReactionAsync(one);
            
            async Task Two()
            {
                await One();
                await msg.AddReactionAsync(two);
            }
            
            async Task Three()
            {
                await Two();
                await msg.AddReactionAsync(three);
            }
            
            async Task Four()
            {
                await Three();
                await msg.AddReactionAsync(four);
            }
            
            async Task Five()
            {
                await Four();
                await msg.AddReactionAsync(five);
            }
            
        }
    }
}