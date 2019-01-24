using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Core.Modules;

namespace Volte.Helpers {
    public static class Utils {
        /// <summary>
        ///     Creates an embed.
        /// </summary>
        /// <param name="ctx">So we can set the Author and Embed Colour of the embed.</param>
        /// <param name="content">Embed content.</param>
        /// <returns>Built EmbedBuilder</returns>
        public static Embed CreateEmbed(VolteContext ctx, object content) {
            var config = ServerConfig.Get(ctx.Guild);
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithDescription(content.ToString())
                .Build();
        }


        /// <summary>
        ///     Sends the passed in Embed to the passed in SocketMessageChannel.
        /// </summary>
        /// <param name="c">Channel to send to</param>
        /// <param name="e">Embed to send</param>
        /// <returns></returns>
        public static async Task Send(IMessageChannel c, Embed e) {
            await c.SendMessageAsync(string.Empty, false, e);
        }

        /// <summary>
        ///     Sends the passed in String to the passed in SocketMessageChannel.
        /// </summary>
        /// <param name="c">Channel to send to</param>
        /// <param name="m">Message to send</param>
        /// <returns></returns>
        public static async Task Send(IMessageChannel c, string m) {
            await c.SendMessageAsync(m);
        }

        public static async Task React(SocketUserMessage m, string rawEmoji) {
            await m.AddReactionAsync(new Emoji(rawEmoji));
        }
    }
}