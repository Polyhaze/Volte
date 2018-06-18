using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Helpers;

namespace SIVA.Core.Runtime
{
    class Program
    {
        public DiscordSocketClient Client;
        
        static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            return;
        }

        private async Task StartAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Verbose});
            await Client.LoginAsync(TokenType.Bot, Config.conf.Token);
            await Client.StartAsync();
            await Client.SetGameAsync(Config.conf.Game, $"https://twitch.tv/{Config.conf.Streamer}", ActivityType.Streaming);
            await Client.SetStatusAsync(UserStatus.Online);
            Client.Log += Log;
            await Task.Delay(-1);


        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
