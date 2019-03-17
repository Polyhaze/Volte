using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Data;

namespace Volte.Extensions
{
    public static class EmbedExtensions
    {
        public static Task<DiscordMessage> SendToAsync(this DiscordEmbedBuilder e, DiscordChannel c) =>
            c.SendMessageAsync(string.Empty, false, e.Build());

        public static Task<DiscordMessage> SendToAsync(this DiscordEmbed e, DiscordChannel c) =>
            c.SendMessageAsync(string.Empty, false, e);

        public static async Task<DiscordMessage> SendToAsync(this DiscordEmbedBuilder e, DiscordMember u) =>
            await (await u.CreateDmChannelAsync()).SendMessageAsync(string.Empty, false, e.Build());

        public static async Task<DiscordMessage> SendToAsync(this DiscordEmbed e, DiscordMember u) =>
            await (await u.CreateDmChannelAsync()).SendMessageAsync(string.Empty, false, e);

        public static DiscordEmbedBuilder WithSuccessColor(this DiscordEmbedBuilder e) =>
            e.WithColor(new DiscordColor((int) Config.SuccessColor));

        public static DiscordEmbedBuilder WithErrorColor(this DiscordEmbedBuilder e) =>
            e.WithColor(new DiscordColor((int) Config.ErrorColor));

        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder e, DiscordUser user) =>
            e.WithAuthor(user.ToHumanReadable(), null, user.AvatarUrl);

        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder e, DiscordMember user) =>
            e.WithAuthor(user.ToHumanReadable(), null, user.AvatarUrl);
    }
}