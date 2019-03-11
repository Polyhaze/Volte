using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Utils;

namespace Volte.Extensions
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

        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, IGuildUser c)
        {
            return await Util.Send(await c.GetOrCreateDMChannelAsync(), e.Build());
        }

        public static async Task<IUserMessage> SendToAsync(this Embed e, IGuildUser c)
        {
            return await Util.Send(await c.GetOrCreateDMChannelAsync(), e);
        }
    }
}