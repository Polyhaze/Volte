using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Helpers;

namespace Volte.Core.Extensions {
    public static class EmbedExtensions {
        public static async Task<IUserMessage> SendTo(this EmbedBuilder e, IMessageChannel c) {
            return await Utils.Send(c, e.Build());
        }

        public static async Task<IUserMessage> SendTo(this Embed e, IMessageChannel c) {
            return await Utils.Send(c, e);
        }
        
        public static async Task<IUserMessage> SendTo(this EmbedBuilder e, SocketGuildUser c) {
            return await Utils.Send(await c.GetOrCreateDMChannelAsync(), e.Build());
        }

        public static async Task<IUserMessage> SendTo(this Embed e, SocketGuildUser c) {
            return await Utils.Send(await c.GetOrCreateDMChannelAsync(), e);
        }
    }
}