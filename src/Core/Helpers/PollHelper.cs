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

            embedBuilder.WithTitle(pollInfo.Prompt);
            embedBuilder.WithFooter(PollInfo.Footer);

            return embedBuilder;
        }
    }
}