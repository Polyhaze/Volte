using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot
{
    internal class Blacklist
    {
        protected static DiscordSocketClient _client = Program._client;

        public static async Task CheckMessageForBlacklistedTerms(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            var config = GuildConfig.GetGuildConfig(context.Guild.Id);
            if (msg == null || context.User.IsBot || config == null) return;
            foreach (var word in config.Blacklist)
            {
                if (msg.Content.Contains(word))
                {
                    await msg.DeleteAsync();
                    break;
                }
            }
        }
    }
}
