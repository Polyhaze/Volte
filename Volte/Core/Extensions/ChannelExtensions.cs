using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Volte.Core.Extensions
{
    public static class ChannelExtensions
    {
        public static Task SendEmbedAsync(this SocketTextChannel tc, Embed e)
        {
            return e.SendTo(tc);
        }
    }
}
