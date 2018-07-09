using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Helpers
{
    public static class Utils
    {
        /// <summary>
        ///     Creates an embed.
        /// </summary>
        /// <param name="ctx">So we can set the Author and Embed Colour of the embed.</param>
        /// <param name="content">Embed content.</param>
        /// <returns>Built EmbedBuilder</returns>
        
        public static Embed CreateEmbed(SocketCommandContext ctx, string content)
        {
            var config = ServerConfig.Get(ctx.Guild);
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content)
                .Build();
        }

        /// <summary>
        ///     Checks if the user given is the bot owner.
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <returns>true; if the user is the bot owner.</returns>
        
        public static bool IsBotOwner(SocketUser user)
        {
            return user.Id == Config.conf.Owner;
        }
    }
}