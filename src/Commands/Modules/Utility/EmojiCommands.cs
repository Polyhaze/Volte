using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("BigEmoji", "HugeEmoji", "BigEmote", "HugeEmote")]
        [Description("Shows the image URL for a given emoji or emote.")]
        public Task<ActionResult> BigEmojiAsync(
            [Description("The emote/emoji you want to see large. Can be a custom emote or a standard Discord emoji.")]
            IEmote emoteIn)
            => Ok(GenerateEmbed(emoteIn));

        [Command("Emotes")]
        [Description("Shows pages for every emote in this guild.")]
        public Task<ActionResult> EmotesAsync()
        {
            var embeds = Context.Guild.Emotes.Select(GenerateEmbed).ToList();
            if (embeds.IsEmpty()) return BadRequest("This guild doesn't have any emotes.");
            return embeds.Count is 1 ? Ok(embeds.First()) : Ok(embeds);
        }

        public EmbedBuilder GenerateEmbed(IEmote emoteIn)
            => emoteIn switch
            {
                Emote emote => Context.CreateEmbedBuilder(Format.Url("Direct Link", emote.Url))
                    .AddField("Created", emote.CreatedAt.FormatBoldString(), true)
                    .AddField("Animated?", emote.Animated ? "Yes" : "No")
                    .WithImageUrl(emote.Url)
                    .WithAuthor($":{emote.Name}:", emote.Url),
                Emoji emoji => Context.CreateEmbedBuilder(Format.Url("Direct Link", emoji.GetUrl()))
                    .AddField("Raw", Format.Code(emoji.Name))
                    .WithImageUrl(emoji.GetUrl()),
                _ => throw new ArgumentException("GenerateEmbed's parameter must be an Emote or an Emoji.")
            };
    }
}