using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;

namespace Volte.Core.Services {
    public class BlacklistService {
        public async Task CheckMessage(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SocketCommandContext(VolteBot.Client, msg);
            var config = ServerConfig.Get(ctx.Guild);

            foreach (var word in config.Blacklist) {
                if (msg.Content.ToLower().Contains(word.ToLower())) {
                    await msg.DeleteAsync();
                    break;
                }
            }
        }
    }
}