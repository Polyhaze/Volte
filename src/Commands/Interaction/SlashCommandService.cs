using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Qmmands.Delegates;
using Volte.Commands.Interaction;
using static Discord.ApplicationCommandType;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public class SlashCommandService : IVolteService
    {
        public static bool UpdateCommandsOnStart { get; set; }
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _provider;

        public SlashCommandService(DiscordShardedClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        private readonly HashSet<ApplicationCommand> _registeredCommands = new HashSet<ApplicationCommand>();

        private HashSet<ApplicationCommand> GetRegisteredCommandsOfType(ApplicationCommandType type)
            => _registeredCommands.Where(x => x.CommandType == type).ToHashSet();

        public HashSet<ApplicationCommand> RegisteredSlashCommands => GetRegisteredCommandsOfType(Slash);
        public HashSet<ApplicationCommand> RegisteredUserCommands => GetRegisteredCommandsOfType(User);
        public HashSet<ApplicationCommand> RegisteredMessageCommands => GetRegisteredCommandsOfType(Message);

        public void RegisterCommands(params ApplicationCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => _registeredCommands.Add(c));

        public void DeregisterCommands(params ApplicationCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => _registeredCommands.Remove(c));

        public async Task<IReadOnlyCollection<
#if DEBUG
            RestGuildCommand
#else
            RestGlobalCommand
#endif
        >> UpdateCommandsAsync()
        {
#if DEBUG
            await _client.Rest.DeleteAllGlobalCommandsAsync();
#endif

            var slashCommands = RegisteredSlashCommands
                .Select(x => x.GetSlashBuilder(_provider)
                    .WithName(x.Name)
                    .WithDescription(x.Description)
                    .Build())
                .ToArray();
            var userCommands = RegisteredUserCommands
                .Select(x => new UserCommandBuilder()
                    .WithName(x.Name)
                    .Build())
                .ToArray();
            var messageCommands = RegisteredMessageCommands
                .Select(x => new MessageCommandBuilder()
                    .WithName(x.Name)
                    .Build())
                .ToArray();

            var commands = slashCommands.Concat<ApplicationCommandProperties>(userCommands).Concat(messageCommands)
                .ToArray();

            return
#if DEBUG
                await _client.Rest.BulkOverwriteGuildCommands(commands, _client.GetPrimaryGuild().Id);
#else
                await _client.Rest.BulkOverwriteGlobalCommands(commands);
#endif
        }


        public async Task InitAsync()
        {
            await Task.Yield();

            foreach (var type in Assembly.GetExecutingAssembly().GetExportedTypes().Where(x =>
                x.Inherits<ApplicationCommand>() && !x.IsAbstract && !x.ContainsGenericParameters))
            {
                try
                {
                    RegisterCommands(Activator.CreateInstance(type).Cast<ApplicationCommand>());
                }
                catch
                { /* ignored */
                }
            }

            _client.InteractionCreated += async interaction =>
            {
                await Task.Yield();
                Executor.Execute(async () =>
                {
#pragma warning disable 8509 | won't happen; there's only 4 non-abstract implementations of SocketInteraction.
                    await (interaction switch
#pragma warning restore 8509
                    {
                        SocketSlashCommand slashCommand => HandleSlashCommandAsync(slashCommand),
                        SocketUserCommand userCommand => HandleUserCommandAsync(userCommand),
                        SocketMessageCommand messageCommand => HandleMessageCommandAsync(messageCommand),
                        SocketMessageComponent messageComponent => HandleMessageComponentAsync(messageComponent)
                    });
                });
            };
        }

        private bool TryGetCommand(string name, out ApplicationCommand command)
        {
            command = _registeredCommands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
            return command != null;
        }

        private async Task HandleUserCommandAsync(SocketUserCommand command)
        {
            if (TryGetCommand(command.Data.Name, out var targetCommand))
                await targetCommand.HandleUserCommandAsync(new UserCommandContext(command, _provider));
            
        }

        private async Task HandleMessageCommandAsync(SocketMessageCommand command)
        {
            if (TryGetCommand(command.Data.Name, out var targetCommand))
                await targetCommand.HandleMessageCommandAsync(new MessageCommandContext(command, _provider));
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            if (TryGetCommand(command.Data.Name, out var targetCommand))
                await targetCommand.HandleSlashCommandAsync(new SlashCommandContext(command, _provider));
        }

        private async Task HandleMessageComponentAsync(SocketMessageComponent component)
        {
            var ctx = new MessageComponentContext(component, _provider);
            if (TryGetCommand(ctx.CustomIdParts.First(), out var targetCommand))
                await targetCommand.HandleComponentAsync(ctx);
        }
    }
}