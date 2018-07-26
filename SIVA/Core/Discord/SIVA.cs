using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class SIVA
    {
        private static DiscordSocketClient Instance = DiscordLogin.Client;

        public static DiscordSocketClient GetInstance => Instance;
        
        
        public SIVA() {
            new Log().PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}