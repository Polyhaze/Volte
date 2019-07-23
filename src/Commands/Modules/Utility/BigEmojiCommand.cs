using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("BigEmoji", "HugeEmoji")]
        [Description("Shows the image URL for a given emoji.")]
        [Remarks("Usage: |prefix|bigemoji {emoji}")]
        public Task<VolteCommandResult> BigEmojiAsync(IEmote emoteIn)
        {
            switch (emoteIn)
            {
                case Emote emote:
                    return Ok(Context.CreateEmbedBuilder(emote.Url).WithImageUrl(emote.Url));

                case Emoji emoji:
                    var url = "https://i.kuro.mu/emoji/512x512/" + string.Join("-",
                                  emoji.ToString().GetUnicodePoints().Select(x => x.ToString("x2"))) +
                              ".png";
                    return Ok(Context.CreateEmbedBuilder(url).WithImageUrl(url));
                default:
                    return None();
            }
        }
    }
}