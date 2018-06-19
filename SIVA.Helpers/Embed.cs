using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files;

namespace SIVA.Helpers
{
    public class Utils
    {
        public Embed CreateEmbed(SocketCommandContext ctx, string content, SocketUser author = null)
        {
            var config = ServerConfig.GetOrCreate(ctx.Guild.Id);
            return new EmbedBuilder()
                .WithAuthor(author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content)
                .Build();
        }
    }
}