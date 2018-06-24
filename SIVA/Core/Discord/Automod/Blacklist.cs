using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord.Automod
{
    public static class Blacklist
    {
        public static async Task CheckMessageForBlacklistedWords(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var ctx = new SocketCommandContext(Program.Client, msg);
            var config = ServerConfig.GetOrCreate(ctx.Guild.Id);

            foreach (var word in config.Blacklist)
            {
                if (msg.Content.ToLower().Contains(word.ToLower()))
                {
                    await msg.DeleteAsync();
                    break;
                }
                    
            }
        }
    }
}