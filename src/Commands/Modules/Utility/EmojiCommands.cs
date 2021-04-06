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
        [Description("Shows the image URL for a given emoji or emote.")]
        public Task<ActionResult> BigEmojiAsync([Description("The emote/emoji you want to see large. Can be a custom emote or a standard Discord emoji.")] IEmote emoteIn) 
            => emoteIn switch
            {
                Emote emote => Ok(GenerateEmoteEmbed(emote)),
                Emoji emoji => Ok(GenerateEmojiEmbed(emoji)),
                _ => None()
            };

        [Command("Emotes")]
        [Description("Shows pages for every emote in this guild.")]
        public Task<ActionResult> EmotesAsync()
        {
            var embeds = Context.Guild.Emotes.Select(GenerateEmoteEmbed).ToList();
            if (embeds.IsEmpty()) return BadRequest("This guild doesn't have any emotes.");
            if (embeds.Count is 1) return Ok(embeds.First());
            return Ok(embeds);
        }

        private EmbedBuilder GenerateEmoteEmbed(Emote emote) =>
            Context.CreateEmbedBuilder(Format.Url("Direct Link", emote.Url)).WithImageUrl(emote.Url);

        private EmbedBuilder GenerateEmojiEmbed(Emoji emoji)
        {
            var url = $"https://i.kuro.mu/emoji/512x512/{emoji.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";
            return Context.CreateEmbedBuilder(Format.Url("Direct Link", url)).WithImageUrl(url);
        }
    }
}