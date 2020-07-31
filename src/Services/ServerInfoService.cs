using System;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class ServerInfoService : BrackeysBotService, IInitializeableService
    {
        private readonly DiscordSocketClient _discord;
        private readonly DataService _dataService;
        private readonly LoggingService _loggingService;

        private Timer _timer;
        private BotConfiguration _config;

        public ServerInfoService(DiscordSocketClient discord, DataService dataService, LoggingService loggingService)
        {
            _discord = discord;
            _dataService = dataService;
            _loggingService = loggingService;
        }

        public void Initialize() 
        {
            _config = _dataService.Configuration;
            _timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            _timer.Elapsed += async (s, e) => await UpdateCategoryCount();
            _timer.Start();
        }

        private async Task UpdateCategoryCount() 
        {
            if (ClientUpAndRunning() && CategoryConfigurationAvailable()) 
            {
                int memberCount = _discord.GetGuild(_config.GuildID).MemberCount;
                ICategoryChannel channel = _discord.GetChannel(_config.InfoCategoryId) as ICategoryChannel;
                string categoryName = _config.InfoCategoryDisplay.Replace("%s%", $"{memberCount}");

                if (CategoryShouldUpdate(channel, categoryName))
                {
                    await channel.ModifyAsync(x => x.Name = categoryName);
                }
            } 
            else 
                await _loggingService.LogMessageAsync(new LogMessage(LogSeverity.Verbose, "ServerInfoService", $"Discord is {_discord}, Guild is {_discord.GetGuild(_config.GuildID)}, InfoCategory is {_config.InfoCategoryDisplay}, InfoCategoryId is {_config.InfoCategoryId}"));
        }

        private bool CategoryShouldUpdate(ICategoryChannel category, string name) 
        {
            return category != null && category.Name != null && !category.Name.Equals(name);
        }

        private bool ClientUpAndRunning() 
        {
            // This service is active before the client is initialized, we should check for client and guild to be available
            return _discord != null && _discord.GetGuild(_config.GuildID) != null;
        }

        private bool CategoryConfigurationAvailable() 
        {
            // Config will likely be set once the bot is running, this should prevent unexpected behaviour
            return _config.InfoCategoryDisplay != null && _config.InfoCategoryId > 0;
        }
    }
}