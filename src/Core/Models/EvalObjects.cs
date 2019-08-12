using System;
using System.Linq;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Core.Models
{
    public sealed class EvalObjects
    {
        internal EvalObjects() { }

        public VolteContext Context { get; set; }
        public DiscordShardedClient Client { get; set; }
        public GuildData Data { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public EmojiService EmojiService { get; set; }

        public SocketGuildUser User(ulong id) 
            => Context.Guild.GetUser(id);

        public SocketGuildUser User(string username) 
            => Context.Guild.Users.FirstOrDefault(a => a.Username.EqualsIgnoreCase(username) || (a.Nickname != null && a.Nickname.EqualsIgnoreCase(username)));

        public SocketTextChannel TextChannel(ulong id) 
            => Context.Guild.GetTextChannel(id);

        public SocketTextChannel TextChannel(string name) 
            => Context.Guild.TextChannels.FirstOrDefault(a => a.Name.EqualsIgnoreCase(name));

        public SocketUserMessage Message(ulong id) 
            => Context.Channel.GetCachedMessage(id) as SocketUserMessage;

        public SocketUserMessage Message(string id)
        {
            if (ulong.TryParse(id, out var ulongId))
            {
                return Message(ulongId);
            }
            throw new ArgumentException($"Method parameter {nameof(id)} is not a valid {typeof(ulong).FullName}.");
        }

    }
}