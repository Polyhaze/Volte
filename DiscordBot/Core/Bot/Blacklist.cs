using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot
{
    internal class Blacklist
    {
        protected static DiscordSocketClient _client;

        public static async Task CheckMessageForBlacklistedTerms(SocketMessage s)
        {
            var offendingAccount = UserAccounts.GetAccount(s.Author);
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;
            var config = GuildConfig.GetGuildConfig(context.Guild.Id);
            if (config == null) return;
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
