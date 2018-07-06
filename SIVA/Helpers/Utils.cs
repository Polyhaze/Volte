using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Helpers
{
    public class Utils
    {
        public static Embed CreateEmbed(SocketCommandContext ctx, string content)
        {
            var config = ServerConfig.Get(ctx.Guild);
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content)
                .Build();
        }

        public static bool IsBotOwner(SocketUser user)
        {
            return user.Id == Config.conf.Owner;
        }
    }
}