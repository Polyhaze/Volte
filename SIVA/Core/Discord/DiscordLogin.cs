using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord
{
    public class DiscordLogin
    {
        public static DiscordSocketClient Client;
        private static readonly SIVAHandler Handler = new SIVAHandler();
        
        public static async Task LoginAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Verbose});
            await Client.LoginAsync(TokenType.Bot, Config.GetToken());
            await Client.StartAsync();
            await Client.SetGameAsync(Config.GetGame(), $"https://twitch.tv/{Config.GetStreamer()}", ActivityType.Streaming);
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