using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord.Automation
{
    public class Antilink
    {
        public static async Task CheckMessageForInvite(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var author = (SocketGuildUser) msg.Author;
            var ctx = new SocketCommandContext(DiscordLogin.Client, msg);
            var config = ServerConfig.Get(ctx.Guild);
            if ((msg.Content.Contains("dis.gd/") 
                 || msg.Content.Contains("discord.gg/") 
                 || msg.Content.Contains("discord.io/") 
                 || msg.Content.Contains("discord.me/") 
                 || msg.Content.Contains("discordapp.com/invite/")) 
                && config.Antilink 
                && !author
                    .Roles
                    .Contains(ctx
                        .Guild
                        .Roles
                        .FirstOrDefault(r => r.Id == config.AdminRole)))
            {
                await msg.DeleteAsync();
            }
        }
    }
}