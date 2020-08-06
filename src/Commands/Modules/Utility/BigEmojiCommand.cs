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
            string url = null;
            try
            {
                url =
                    $"https://i.kuro.mu/emoji/512x512/{emoteIn.Cast<Emoji>()?.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            }
            catch (ArgumentNullException)
            { }

            return emoteIn switch
            {
                Emote emote => Ok(Context.CreateEmbedBuilder(emote.Url).WithImageUrl(emote.Url)),
                Emoji _ => Ok(Context.CreateEmbedBuilder(url).WithImageUrl(url)),
                _ => None() //should never be reached
            };
        }
    }
}