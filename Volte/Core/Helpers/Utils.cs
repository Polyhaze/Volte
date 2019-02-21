using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Commands;
using Volte.Core.Data;

namespace Volte.Core.Helpers
{
    public static class Utils
    {
        /// <summary>
        ///     Creates an embed.
        /// </summary>
        /// <param name="ctx">So we can set the Author and Embed Colour of the embed.</param>
        /// <param name="content">Embed content.</param>
        /// <returns>Built EmbedBuilder</returns>
        public static Embed CreateEmbed(VolteContext ctx, object content)
        {
            return new EmbedBuilder()
                .WithAuthor(ctx.Message.Author)
                .WithColor(Config.GetSuccessColor())
                .WithDescription(content.ToString())
                .Build();
        }


        /// <summary>
        ///     Sends the passed in Embed to the passed in SocketMessageChannel.
        /// </summary>
        /// <param name="c">Channel to send to</param>
        /// <param name="e">Embed to send</param>
        /// <returns></returns>
        public static async Task<IUserMessage> Send(IMessageChannel c, Embed e)
        {
            return await c.SendMessageAsync(string.Empty, false, e);
        }

        /// <summary>
        ///     Sends the passed in String to the passed in SocketMessageChannel.
        /// </summary>
        /// <param name="c">Channel to send to</param>
        /// <param name="m">Message to send</param>
        /// <returns></returns>
        public static async Task<IUserMessage> Send(IMessageChannel c, string m)
        {
            return await c.SendMessageAsync(m);
        }

        public static async Task React(SocketUserMessage m, string rawEmoji)
        {
            await m.AddReactionAsync(new Emoji(rawEmoji));
        }
    }
}