using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("BigEmoji", "HugeEmoji", "BigEmote", "HugeEmote")]
        [Description("Shows the image URL for a given emoji.")]
        [Remarks("bigemoji {Emote}")]
        public Task<ActionResult> BigEmojiAsync(IEmote emoteIn)
        {
            string url;
            try
            {
                url = $"https://i.kuro.mu/emoji/512x512/{emoteIn.Cast<Emoji>()?.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            }
            catch (ArgumentNullException)
            {
                url = emoteIn.Cast<Emote>().Url;
            }

            return Ok(Context.CreateEmbedBuilder(url).WithImageUrl(url));
        }
    }
}