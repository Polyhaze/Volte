using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Support
{
    public class SupportMessageListener
    {
        public async Task Check(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var ctx = new SocketCommandContext(DiscordLogin.Client, msg);
            var config = ServerConfig.Get(ctx.Guild);
            
        }
        
    }
}