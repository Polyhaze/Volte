/*using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("BigEmoji", "HugEmoji")]
        [Description("Shows the image URL for a given emoji.")]
        [Remarks("Usage: |prefix|bigemoji {emoji}")]
        public async Task BigEmojiAsync(DiscordEmoji emoteIn)
        {
            switch (emoteIn)
            {
                case DiscordGuildEmoji emote:
                    await Context.CreateEmbedBuilder(emote).WithImageUrl(emote.Url).SendToAsync(Context.Channel);
                    break;
                case DiscordEmoji emoji:
                    var url = "https://i.kuro.mu/emoji/512x512/" + string.Join("-",
                                  emoji.ToString().GetUnicodePoints().Select(x => x.ToString("x2"))) +
                              ".png";
                    await Context.CreateEmbedBuilder(url).WithImageUrl(url).SendToAsync(Context.Channel);
                    break;
            }
        }
    }
}*/
