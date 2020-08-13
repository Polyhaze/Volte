using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Volte.Commands;
using Volte.Core.Models.Misc;
using Volte.Services;

namespace Volte.Core.Helpers
{
    public static class PollHelpers
    {
        public static async Task<Dictionary<string, int>> GetPollVotesAsync(VolteContext ctx, ulong id, int optionsLength)
        {
            var reactions =
                (await (await (await ctx.Client.GetShardFor(ctx.Guild).Rest.GetGuildAsync(ctx.Guild.Id))
                    .GetTextChannelAsync(
                        ctx.Channel.Id)).GetMessageAsync(id)).Reactions;
            
            var one = reactions.Count(x => x.Key.Name == EmojiHelper.One && !x.Value.IsMe);
            var two = reactions.Count(x => x.Key.Name == EmojiHelper.Two && !x.Value.IsMe);
            var three = reactions.Count(x => x.Key.Name == EmojiHelper.Three && !x.Value.IsMe);
            var four = reactions.Count(x => x.Key.Name == EmojiHelper.Four && !x.Value.IsMe);
            var five = reactions.Count(x => x.Key.Name == EmojiHelper.Five && !x.Value.IsMe);
            return optionsLength switch
            {
                1 => new Dictionary<string, int> {{EmojiHelper.One, one}},
                2 => new Dictionary<string, int> {{EmojiHelper.One, one}, {EmojiHelper.Two, two}},
                3 => new Dictionary<string, int>
                {
                    {EmojiHelper.One, one}, {EmojiHelper.Two, two}, {EmojiHelper.Three, three}
                },
                4 => new Dictionary<string, int>
                {
                    {EmojiHelper.One, one},
                    {EmojiHelper.Two, two},
                    {EmojiHelper.Three, three},
                    {EmojiHelper.Four, four}
                },
                5 => new Dictionary<string, int>
                {
                    {EmojiHelper.One, one},
                    {EmojiHelper.Two, two},
                    {EmojiHelper.Three, three},
                    {EmojiHelper.Four, four},
                    {EmojiHelper.Five, five}
                },
                _ => new Dictionary<string, int>()
            };
        }

        public static PollInfo GetPollBody(IEnumerable<string> choices)
        {
            var c = choices as string[] ?? choices.ToArray();
            return PollInfo.FromDefaultFields(c.Length - 1, c);
        }

        public static Task AddPollReactionsAsync(int amount, IUserMessage msg)
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