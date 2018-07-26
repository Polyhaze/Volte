using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class SIVA {

        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        public SIVA() {
            new Log().PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}