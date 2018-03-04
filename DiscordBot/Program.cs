using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace SIVA
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private EventHandler _handler;

        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(SIVA.Config.bot.Token))
            {
                Console.WriteLine("Token not specified.");
                Console.ReadLine();
                return;
            }
            _client = new DiscordSocketClient(new DiscordSocketConfig{LogLevel = LogSeverity.Verbose});
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, SIVA.Config.bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync(SIVA.Config.bot.BotGameToSet, $"https://twitch.tv/{SIVA.Config.bot.TwitchStreamer}", StreamType.Twitch);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            _handler = new EventHandler();
            await _handler.InitializeAsync(_client);
            Console.WriteLine("Use this to invite the bot into your server: https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8");
            await Task.Delay(-1);
        }

        private async static Task Log(LogMessage msg)
        {
            if (!SIVA.Config.bot.Debug) return;
            Console.WriteLine("INFO: " + msg.Message);
            try
            {
                File.AppendAllText("Debug.log", $"{msg.Message}\n");
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText("Debug.log", msg.Message);
            }
        }
    }
}
