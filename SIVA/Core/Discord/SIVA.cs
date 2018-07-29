using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class Siva {

        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        public Siva() {
            new Log().PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}