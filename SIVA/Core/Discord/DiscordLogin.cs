using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SIVA.Core.Discord.Automation;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    public class DiscordLogin {
        
        public static DiscordSocketClient Client;
        public static readonly SIVAHandler Handler = new SIVAHandler();
        public static readonly Log Logger = new Log();

        public static async Task LoginAsync() {
            Client = new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Verbose});
            await Client.LoginAsync(TokenType.Bot, Config.GetToken());
            await Client.StartAsync();
            await Client.SetGameAsync(Config.GetGame(), $"https://twitch.tv/{Config.GetStreamer()}",
                ActivityType.Streaming);
            await Client.SetStatusAsync(UserStatus.Online);
            await Handler.Init();
            Client.Log += Log;
            await Task.Delay(-1);
        }

        private static async Task Log(LogMessage msg) {
            switch (msg.Severity) {
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                    Logger.Info(msg.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.Warn(msg.Message);
                    break;
                case LogSeverity.Error:
                    Logger.Error(msg.Message);
                    break;
                case LogSeverity.Critical:
                    Logger.Error(msg.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.Debug(msg.Message);
                    break;
                default:
                    throw new InvalidDataException();
            }
        }
        
    }
}