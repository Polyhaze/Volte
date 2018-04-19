using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Discord;

namespace SIVA.Core.Bot
{
    internal class Program
    {
        
        public static DiscordSocketClient _client;
        public static EventHandler _handler;

        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.bot.Token)) 
            {
                InteractiveSetup.Setup();
            }

            LogSeverity logSeverity;
            switch (Config.bot.LogSeverity)
            {
                case "Verbose":
                case "verbose":
                    logSeverity = LogSeverity.Verbose;
                    break;
                case "Info":
                case "info":
                    logSeverity = LogSeverity.Info;
                    break;
                case "Warning":
                case "warning":
                    logSeverity = LogSeverity.Warning;
                    break;
                case "Debug":
                case "debug":
                    logSeverity = LogSeverity.Debug;
                    break;
                case "Critical":
                case "critical":
                    logSeverity = LogSeverity.Critical;
                    break;
                case "Error":
                case "error":
                    logSeverity = LogSeverity.Error;
                    break;
                default:
                    logSeverity = LogSeverity.Verbose;
                    break;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = logSeverity} );
            await _client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync(Config.bot.BotGameToSet, $"https://twitch.tv/{Config.bot.TwitchStreamer}", StreamType.Twitch);
            await _client.SetStatusAsync(UserStatus.Online);
            _client.Log += EventUtils.Log;
            _handler = new EventHandler();
            await _handler.InitializeAsync(_client);
            Console.WriteLine("Public SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=320942091049893888&permissions=8");
            Console.WriteLine("Dev SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8");
            //SivaPanel.StartPanel(); this method isnt needed for a while
            await Task.Delay(-1);
        }
    }
}
