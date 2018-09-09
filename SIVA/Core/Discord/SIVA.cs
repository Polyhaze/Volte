using Discord.WebSocket;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class SIVA {

        public static string GetPatreonLink() => "https://patreon.com/_SIVA";
        public static Log GetLogger() => Log.GetLogger();
        public static SIVAHandler GetEventHandler() => DiscordLogin.Handler;
        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        public SIVA() {
            GetLogger().PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}