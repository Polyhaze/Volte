using System;
using System.Net.Http.Headers;
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
        public static readonly SIVAHandler Handler = new SIVAHandler();
        
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
            if (msg.Severity == LogSeverity.Info)
            {
                new Log().Info(msg.Message);
            }

            if (msg.Severity == LogSeverity.Verbose)
            {
                new Log().Info(msg.Message);
            }

            if (msg.Severity == LogSeverity.Warning)
            {
                new Log().Warn(msg.Message);
            }

            if (msg.Severity == LogSeverity.Error)
            {
                new Log().Fatal(msg.Message);
            }
        }
        
    }
}