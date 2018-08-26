using Discord.WebSocket;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class SIVA {

        public static string PatreonLink => "https://patreon.com/_SIVA";
        public static Log Logger => Log.GetLogger();
        public static SIVAHandler EventHandler => DiscordLogin.Handler;
        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        public SIVA() {
            Logger.PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
        
    }
}