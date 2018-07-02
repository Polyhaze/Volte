using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord
{
    public class DiscordLogin
    {
        public static DiscordSocketClient Client;
        public static EventHandler Handler = new EventHandler();
        
        public static async Task LoginAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Verbose});
            await Client.LoginAsync(TokenType.Bot, Config.conf.Token);
            await Client.StartAsync();
            await Client.SetGameAsync(Config.conf.Game, $"https://twitch.tv/{Config.conf.Streamer}", ActivityType.Streaming);
            await Client.SetStatusAsync(UserStatus.Online);
            await Handler.Init();
            Client.Log += Log;
            await Task.Delay(-1);
        }
        
        public static async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}