using System;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    public class BrackeysBotContext : ICommandContext
    {
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }

        public DataService DataService => _dataService;
        public BotConfiguration Configuration => _dataService.Configuration;

        private readonly DataService _dataService;

        public BrackeysBotContext(SocketUserMessage msg, IServiceProvider provider)
        {
            Client = (IDiscordClient)provider.GetService(typeof(DiscordSocketClient));
            Channel = msg.Channel;
            Guild = (Channel as SocketTextChannel)?.Guild;
            User = msg.Author;
            Message = msg;

            _dataService = provider.GetService(typeof(DataService)) as DataService;
        }
    }
}
