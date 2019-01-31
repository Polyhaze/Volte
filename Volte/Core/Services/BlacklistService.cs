using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Data;

namespace Volte.Core.Services {
    internal class BlacklistService {
        internal async Task CheckMessage(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SocketCommandContext(VolteBot.Client, msg);
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);

            foreach (var word in config.Blacklist) {
                if (msg.Content.ToLower().Contains(word.ToLower())) {
                    await msg.DeleteAsync();
                    break;
                }
            }
        }
    }
}