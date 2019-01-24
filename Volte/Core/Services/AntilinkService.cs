using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;

namespace Volte.Core.Services {
    public class AntilinkService {
        public async Task CheckMessage(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var author = (SocketGuildUser) msg.Author;
            var ctx = new SocketCommandContext(VolteBot.Client, msg);
            var config = ServerConfig.Get(ctx.Guild);
            if ((msg.Content.Contains("dis.gd/")
                 || msg.Content.Contains("discord.gg/")
                 || msg.Content.Contains("discord.io/")
                 || msg.Content.Contains("discord.me/")
                 || msg.Content.Contains("discordapp.com/invite/"))
                && config.Antilink
                && !author
                    .Roles.Contains(ctx.Guild.Roles.FirstOrDefault(r => r.Id == config.AdminRole))) {
                await msg.DeleteAsync();
            }
        }
    }
}