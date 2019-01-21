using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord.Automation {
    public class BlacklistService {
        public async Task CheckMessageForBlacklistedWords(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SocketCommandContext(SIVA.Client, msg);
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