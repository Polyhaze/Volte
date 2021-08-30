using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using SentrySdk = Sentry.SentrySdk;
using Volte.Interactions;
using static Discord.ApplicationCommandType;

namespace Volte.Services
{
    
    public class InteractionService : IVolteService
    {
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _provider;

        public InteractionService(DiscordShardedClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
            CommandUpdater = new CommandUpdatingService(this, provider);
        }
        
        public CommandUpdatingService CommandUpdater { get; }

        public HashSet<ApplicationCommand> AllRegisteredCommands { get; } = new HashSet<ApplicationCommand>();

        private HashSet<ApplicationCommand> GetRegisteredCommandsOfType(ApplicationCommandType type)
            => AllRegisteredCommands.Where(x => x.CommandType == type).ToHashSet();

        public HashSet<ApplicationCommand> RegisteredSlashCommands => GetRegisteredCommandsOfType(Slash);
        public HashSet<ApplicationCommand> RegisteredUserCommands => GetRegisteredCommandsOfType(User);
        public HashSet<ApplicationCommand> RegisteredMessageCommands => GetRegisteredCommandsOfType(Message);

        public void RegisterCommands(params ApplicationCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => AllRegisteredCommands.Add(c));

        public void DeregisterCommands(params ApplicationCommand[] commands) =>
            commands.Where(x => x != null).ForEach(c => AllRegisteredCommands.Remove(c));
        
        public async Task InitAsync()
        {
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
                try
                {
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
                }
                catch (Exception e)
                {
                    SentrySdk.AddBreadcrumb("Error occurred when handling the event of some form of Interaction.");
                    SentrySdk.CaptureException(e);
                }

            };
            
            await CommandUpdater.InitAsync();
            
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
        
        
        
        private bool TryGetCommand(string name, out ApplicationCommand command)
        {
            command = AllRegisteredCommands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
            return command != null;
        }
    }
}