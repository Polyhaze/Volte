using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Services {
    public class BlacklistService {
        public async Task CheckMessage(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SocketCommandContext(Discord.SIVA.Client, msg);
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