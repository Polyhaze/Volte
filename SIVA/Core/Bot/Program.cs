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
        private EventHandler _handler;

        private static void Main()
        {
            Console.Title = "SIVA";
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private async Task StartAsync()
        {
            if (String.IsNullOrEmpty(Config.bot.Token)) 
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
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            _client.Log += EventUtils.Log;
            _handler = new EventHandler();
            await _handler.InitializeAsync(_client);
            Console.WriteLine("Public SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=320942091049893888&permissions=8");
            Console.WriteLine("Dev SIVA: https://discordapp.com/oauth2/authorize?scope=bot&client_id=410547925597421571&permissions=8");
            //SivaPanel.StartPanel(); this method isnt needed for a while
            await Task.Delay(-1);
        }

        /*private async Task ConsoleInput()
        {
            string input = string.Empty;
            while (input.Trim().ToLower() != "stop")
            {
                input = Console.ReadLine();
                _handler._service.ExecuteAsync();
            }
        }

        private void ConsoleSendMessage()
        {
            Console.Write("Select a Guild: ");
            SocketGuild guild = GetSelectedGuild(_client.Guilds);
        }

        private SocketGuild GetSelectedGuild(IEnumerable<SocketGuild> guilds)
        {
            var socketGuilds = guilds.ToList();
            var maxIndex = socketGuilds.Count - 1;
            for (var i = 0; i <= maxIndex; i++)
            {
                Console.WriteLine($"{i} - {socketGuilds[i].Name}");
            }

            var selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                var success = int.TryParse(Console.ReadLine().Trim(), out selectedIndex);
                if (!success) Console.WriteLine("That wasn't a number you shit.");
                if (selectedIndex < 0 || selectedIndex > maxIndex) Console.WriteLine($"That index was below 0 or above {maxIndex}. Try again.");
            }

            return socketGuilds[selectedIndex];
        }*/
    }
}
