using System;
using System.IO;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class LoggingService : BrackeysBotService, IInitializeableService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public string LogDirectory { get; }
        public string LogFile => Path.Combine(LogDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log");

        private static object _lockLogFile = new object();

        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            _discord = discord;
            _commands = commands;
        }
        public void Initialize()
        {
            _discord.Log += LogMessageAsync;
            _commands.Log += LogMessageAsync;
        }

        public Task LogMessageAsync(LogMessage msg)
        {
            // Create the log directory if it doesn't exist
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            // Create today's log file if it doesn't exist
            if (!File.Exists(LogFile))
                File.Create(LogFile).Dispose();

            // Write the log text to a file
            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            lock (_lockLogFile)
            {
                File.AppendAllText(LogFile, logText + "\n");
            }

            // Write the log text to the console
            return Console.Out.WriteLineAsync(logText);
        }
    }
}