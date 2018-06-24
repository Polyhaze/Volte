using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Runtime
{
    internal class Program
    {
        public static DiscordSocketClient Client;
        public static EventHandler Handler = new EventHandler();
        
        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            StartAsync().GetAwaiter().GetResult();
        }

        private static async Task StartAsync()
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
