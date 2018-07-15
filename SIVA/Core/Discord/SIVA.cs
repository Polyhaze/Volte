using Discord.WebSocket;

namespace SIVA.Core.Discord
{
    public class SIVA
    {
        public static DiscordSocketClient Instance = DiscordLogin.Client;
        
        public SIVA() {
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}