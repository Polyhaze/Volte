using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("BigEmoji", "HugeEmoji", "BigEmote", "HugeEmote")]
        [Description("Shows the image URL for a given emoji.")]
        public Task<ActionResult> BigEmojiAsync([Description("The emote/emoji you want to see large. Can be a custom emoji or a standard Discord emoji.")] IEmote emoteIn) 
            => emoteIn switch
            {
                Emote emote => Ok(Context.CreateEmbedBuilder(Format.Url("Direct Link", emote.Url)).WithImageUrl(emote.Url)),
                Emoji emoji => Ok(GenerateEmojiEmbed(emoji)),
                _ => None()
            };

        private EmbedBuilder GenerateEmojiEmbed(Emoji emoji)
        {
            var url = $"https://i.kuro.mu/emoji/512x512/{emoji.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            return Context.CreateEmbedBuilder(Format.Url("Direct Link", url)).WithImageUrl(url);
        }
    }
}