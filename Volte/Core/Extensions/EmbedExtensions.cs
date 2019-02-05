using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Helpers;

namespace Volte.Core.Extensions {
    public static class EmbedExtensions {
        public static async Task SendTo(this EmbedBuilder e, IMessageChannel c) {
            await Utils.Send(c, e.Build());
        }

        public static async Task SendTo(this Embed e, IMessageChannel c) {
            await Utils.Send(c, e);
        }
        
        public static async Task SendTo(this EmbedBuilder e, SocketGuildUser c) {
            await Utils.Send(await c.GetOrCreateDMChannelAsync(), e.Build());
        }

        public static async Task SendTo(this Embed e, SocketGuildUser c) {
            await Utils.Send(await c.GetOrCreateDMChannelAsync(), e);
        }
    }
}