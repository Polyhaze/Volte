using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Discord;
using SIVA.WebPanel.Backend;

namespace SIVA.Core.Bot
{
    internal class Program
    {
        public static DiscordSocketClient _client;
        private EventHandler _handler;

        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.bot.Token)) 
            {
                InteractiveSetup.Setup();
            }

            LogSeverity LogSeverity;
            switch (Config.bot.LogSeverity)
            {
                case "Verbose":
                case "verbose":
                    LogSeverity = LogSeverity.Verbose;
                    break;
                case "Info":
                case "info":
                    LogSeverity = LogSeverity.Info;
                    break;
                case "Warning":
                case "warning":
                    LogSeverity = LogSeverity.Warning;
                    break;
                case "Debug":
                case "debug":
                    LogSeverity = LogSeverity.Debug;
                    break;
                case "Critical":
                case "critical":
                    LogSeverity = LogSeverity.Critical;
                    break;
                case "Error":
                case "error":
                    LogSeverity = LogSeverity.Error;
                    break;
                default:
                    LogSeverity = LogSeverity.Verbose;
                    break;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity} );
            await _client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync(Config.bot.BotGameToSet, $"https://twitch.tv/{Config.bot.TwitchStreamer}", StreamType.Twitch);
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            _client.Log += EventUtils.Log;
            _handler = new EventHandler();
            await _handler.InitializeAsync(_client);
            Console.WriteLine("Public SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=320942091049893888&permissions=8");
            Console.WriteLine("Dev SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8");
            SivaPanel.InitialiseServer(); //start it
            await Task.Delay(-1);
        }
    }
}
