using Discord.WebSocket;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    public class SIVA {
        public static string GetPatreonLink() => "https://patreon.com/_SIVA";
        public static Log GetLogger() => Log.GetLogger();
        public static SIVAHandler GetEventHandler() => DiscordLogin.Handler;
        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        /// <summary>
        ///     WARNING:
        ///     Instantiating this object will start a completely new bot instance.
        ///     Don't do that, unless you're making a restart function!
        /// </summary>
        public SIVA() {
            GetLogger().PrintVersion();
            DiscordLogin
                .LoginAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}