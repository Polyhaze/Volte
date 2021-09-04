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
                Executor.Execute(async () =>
                {
                    try
                    {
                        await (interaction switch
                        {
                            SocketSlashCommand slashCommand => GetCommand(slashCommand.Data.Name)
                                .HandleSlashCommandAsync(new SlashCommandContext(slashCommand, _provider)),

                            SocketUserCommand userCommand => GetCommand(userCommand.Data.Name)
                                .HandleUserCommandAsync(new UserCommandContext(userCommand, _provider)),

                            SocketMessageCommand messageCommand => GetCommand(messageCommand.Data.Name)
                                .HandleMessageCommandAsync(new MessageCommandContext(messageCommand, _provider)),

                            SocketMessageComponent messageComponent => GetCommand(messageComponent.Data.CustomId.Split(':')[0])
                                .HandleComponentAsync(new MessageComponentContext(messageComponent, _provider)),
                            
                            _ => null
                        } ?? Task.CompletedTask);
                    }
                    catch (Exception e)
                    {
                        SentrySdk.AddBreadcrumb("Error occurred when handling the event of some form of Interaction.");
                        SentrySdk.CaptureException(e);
                    }
                });
            };

            await CommandUpdater.InitAsync();
        }


        private ApplicationCommand GetCommand(string name)
            => AllRegisteredCommands.First(x => x.Name.EqualsIgnoreCase(name));
    }
}