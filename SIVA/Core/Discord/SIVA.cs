using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord
{
    public class SIVA
    {
        private static DiscordSocketClient Instance = DiscordLogin.Client;

        public static DiscordSocketClient GetInstance => Instance;
        
        
        public SIVA() {
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}