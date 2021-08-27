using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Volte.Commands.Slash;
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

        private readonly HashSet<SlashCommand> _registeredCommands = new HashSet<SlashCommand>();

        public void RegisterCommands(params SlashCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => _registeredCommands.Add(c));

        public void DeregisterCommands(params SlashCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => _registeredCommands.Remove(c));
        
#if DEBUG
        public async Task<IReadOnlyCollection<RestGuildCommand>> UpdateCommandsAsync()
        {
            await _client.Rest.DeleteAllGlobalCommandsAsync();
            // ReSharper disable twice CoVariantArrayConversion
            return await _client.Rest.BulkOverwriteGuildCommands(_registeredCommands.Select(command =>
                command.GetCommandBuilder(_provider).WithName(command.Name).WithDescription(command.Description)
                    .Build()).ToArray(), _client.GetPrimaryGuild().Id);
        }
#else
        public Task<IReadOnlyCollection<RestGlobalCommand>> UpdateCommandsAsync()
            => _client.Rest.BulkOverwriteGlobalCommands(
                _registeredCommands.Select(command =>
                    command.GetCommandBuilder(_provider).WithName(command.Name).WithDescription(command.Description)
                        .Build()).ToArray());
#endif


        public async Task InitAsync()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetExportedTypes().Where(x =>
                x.Inherits<SlashCommand>() && !x.IsAbstract && !x.ContainsGenericParameters))
            {
                try
                {
                    RegisterCommands(type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>())
                        .Cast<SlashCommand>());
                }
                catch { /* ignored */ }
            }
            
            _client.InteractionCreated += async interaction =>
            {
                await Task.Yield();
                Executor.Execute(async () =>
                {
                    await (interaction switch
                    {
                        SocketSlashCommand slashCommand => HandleSlashCommandAsync(slashCommand),
                        SocketMessageComponent messageComponent => HandleMessageComponentAsync(messageComponent),
                        _ => Task.CompletedTask
                    });
                });
            };


            _client.MessageCommandExecuted += HandleMessageCommandAsync;
            _client.UserCommandExecuted += HandleUserCommandAsync;
        }

        private async Task HandleUserCommandAsync(SocketUserCommand command) { }

        private async Task HandleMessageCommandAsync(SocketMessageCommand command) { }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var targetCommand = _registeredCommands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(command.Data.Name));
            if (targetCommand != null)
                await targetCommand.HandleAsync(new SlashCommandContext(command, _provider));
        }

        private async Task HandleMessageComponentAsync(SocketMessageComponent component)
        {
            var targetCommand = _registeredCommands.FirstOrDefault(x =>
                x.Name.EqualsIgnoreCase(component.Data.CustomId.Split(':').FirstOrDefault()));
            if (targetCommand != null)
                await targetCommand.HandleComponentAsync(new MessageComponentContext(component, _provider));
        }
    }
}