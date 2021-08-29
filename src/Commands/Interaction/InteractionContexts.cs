using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Entities;

namespace Volte.Commands.Interaction
{
    public class SlashCommandContext : InteractionContext<SocketSlashCommand>
    {
        public SlashCommandContext(SocketSlashCommand command, IServiceProvider provider) : base(command, provider) { }

        public SafeDictionary<string, object> ValuedOptions
            => Backing.Data.Options.ToDictionary(x => x.Name, x => x.Value).AsSafe();

        public SafeDictionary<string, SocketSlashCommandDataOption> Options
            => Backing.Data.Options?.ToDictionary(x => x.Name).AsSafe();

        public SocketSlashCommandDataOption GetOption(string name) =>
            Backing.Data.Options?.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
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