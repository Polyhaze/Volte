using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files;

namespace SIVA.Helpers
{
    public class Utils
    {
        public static Embed CreateEmbed(SocketCommandContext ctx, string content)
        {
            var config = ServerConfig.GetOrCreate(ctx.Guild.Id);
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content)
                .Build();
        }
    }
}