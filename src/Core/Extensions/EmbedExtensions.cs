using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Core.Utils;

namespace Volte.Core.Extensions
{
    public static class EmbedExtensions
    {
        public static Task<IUserMessage> SendToAsync(this EmbedBuilder e, IMessageChannel c)
        {
            return Util.Send(c, e.Build());
        }

        public static Task<IUserMessage> SendToAsync(this Embed e, IMessageChannel c)
        {
            return Util.Send(c, e);
        }

        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, SocketGuildUser c)
        {
            return await Util.Send(await c.GetOrCreateDMChannelAsync(), e.Build());
        }

        public static async Task<IUserMessage> SendToAsync(this Embed e, SocketGuildUser c)
        {
            return await Util.Send(await c.GetOrCreateDMChannelAsync(), e);
        }
    }
}