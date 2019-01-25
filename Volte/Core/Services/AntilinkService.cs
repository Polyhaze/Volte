using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Core.Modules;

namespace Volte.Core.Services {
    internal class AntilinkService {
        internal async Task CheckMessage(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var author = (SocketGuildUser) msg.Author;
            var ctx = new VolteContext(VolteBot.Client, msg);
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);
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