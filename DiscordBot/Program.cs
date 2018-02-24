using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace DiscordBot
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private MessageHandler _handler;

        private static void Main()
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            if (String.IsNullOrEmpty(Config.bot.token))
            {
                Console.WriteLine("Token not specified.");
                return;
            }
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            Console.Title = "SIVA Discord bot";
            await _client.SetGameAsync(Config.bot.botGameToSet);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            Console.WriteLine("Use this to invite the bot into your server: https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8");
            _handler = new MessageHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async static Task Log(LogMessage msg)
        {
            if (Config.bot.debug)
            {
                Console.WriteLine("debug: " + msg.Message);
                try
                {
                    File.AppendAllText("Log.txt", $"{msg.Message}\n");
                }
                catch (FileNotFoundException)
                {
                    File.WriteAllText("Log.txt", msg.Message);
                }
            }
        }
    }
}
