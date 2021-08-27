using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Entities;

namespace Volte.Commands.Slash
{
    public class SlashCommandContext : InteractionContext<SocketSlashCommand>
    {
        public SlashCommandContext(SocketSlashCommand command, IServiceProvider provider) : base(command, provider) { }

        public SafeDictionary<string, object> ValuedOptions
            => Options.Values.ToDictionary(x => x.Name, x => x.Value).AsSafe();

        public SafeDictionary<string, SocketSlashCommandDataOption> Options
            => Backing.Data.Options?.ToDictionary(x => x.Name)?.AsSafe() ??
               new SafeDictionary<string, SocketSlashCommandDataOption>();

        public InteractionReplyBuilder<SocketSlashCommand> CreateReplyBuilder(bool ephemeral = false) =>
            new InteractionReplyBuilder<SocketSlashCommand>(this).WithEphemeral(ephemeral);


        public SocketSlashCommandDataOption GetOption(string name) =>
            Backing.Data.Options?.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
    }
}