using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using Gommon;
using Volte.Entities;

namespace Volte.Interactions
{
    public class SlashCommandContext : InteractionContext<SocketSlashCommand>
    {
        public SlashCommandContext(SocketSlashCommand command, IServiceProvider provider) : base(command, provider) { }

        public SafeDictionary<string, object> ValuedOptions => Options
            .ToDictionary(x => x.Key, x => x.Value.Value).AsSafe();

        public SafeDictionary<string, SocketSlashCommandDataOption> Options => Backing.Data.GetOptions();

        public SocketSlashCommandDataOption GetOption(string name) => Options[name];
    }

    public class MessageCommandContext : InteractionContext<SocketMessageCommand>
    {
        public MessageCommandContext(SocketMessageCommand backing, IServiceProvider provider)
            : base(backing, provider) { }

        public SocketUserMessage UserMessage => Backing.Data.Message.Cast<SocketUserMessage>();
    }

    public class UserCommandContext : InteractionContext<SocketUserCommand>
    {
        public UserCommandContext(SocketUserCommand backing, IServiceProvider provider)
            : base(backing, provider) { }

        public SocketGuildUser TargetedGuildUser => Backing.Data.Member.Cast<SocketGuildUser>();
    }

    public class MessageComponentContext : InteractionContext<SocketMessageComponent>
    {
        public MessageComponentContext(SocketMessageComponent backing, IServiceProvider provider)
            : base(backing, provider) { }

        public string CustomId => Backing.Data.CustomId;
        public string[] CustomIdParts => CustomId.Split(':');

        public IEnumerable<string> SelectedMenuOptions
            => Backing.Data.Values?.ToHashSet() ?? new HashSet<string>();
    }
}