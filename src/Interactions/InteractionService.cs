using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using SentrySdk = Sentry.SentrySdk;
using Volte.Interactions;
using static Discord.ApplicationCommandType;

namespace Volte.Services
{
    public class InteractionService : IVolteService
    {
        public InteractionService(DiscordShardedClient client, IServiceProvider provider)
        {
            CommandUpdater = new CommandUpdatingService(this, provider);

            Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(x => x.Inherits<ApplicationCommand>() && !x.IsAbstract && !x.ContainsGenericParameters)
                .ForEach(type =>
                    Lambda.Try(() =>
                        RegisterCommands(Activator.CreateInstance(type).Cast<ApplicationCommand>())
                    ));

            client.InteractionCreated += async interaction =>
            {
                await Task.Yield();
                Executor.Execute(async () =>
                {
                    try
                    {
                        await (interaction switch
                        {
                            SocketSlashCommand slashCommand => GetCommand(slashCommand.Data.Name)?
                                .HandleSlashCommandAsync(new SlashCommandContext(slashCommand, provider)),

                            SocketUserCommand userCommand => GetCommand(userCommand.Data.Name)?
                                .HandleUserCommandAsync(new UserCommandContext(userCommand, provider)),

                            SocketMessageCommand messageCommand => GetCommand(messageCommand.Data.Name)?
                                .HandleMessageCommandAsync(new MessageCommandContext(messageCommand, provider)),

                            SocketMessageComponent messageComponent => GetCommand(
                                    new MessageComponentId(messageComponent.Data.CustomId).Identifier)?
                                .HandleComponentAsync(new MessageComponentContext(messageComponent, provider)),

                            _ => null
                        } ?? Task.CompletedTask);
                    }
                    catch (Exception e)
                    {
                        SentrySdk.AddBreadcrumb("Error occurred when handling the event of some form of Interaction.");
                        Logger.Exception(e);
                    }
                });
            };
        }

        public CommandUpdatingService CommandUpdater { get; }

        public HashSet<ApplicationCommand> AllRegisteredCommands { get; } = new HashSet<ApplicationCommand>();

        public HashSet<ApplicationCommand> AllRegisteredGuildCommands => GetAllGuildOrGlobalCommands(true);
        public HashSet<ApplicationCommand> AllRegisteredGlobalCommands => GetAllGuildOrGlobalCommands(false);

        private HashSet<ApplicationCommand> GetAllGuildOrGlobalCommands(bool guildOnly)
            => AllRegisteredCommands.Where(x =>
                    (!guildOnly && !x.IsLockedToGuild) ||
                    (guildOnly && x.IsLockedToGuild) //yeah, this works properly and i love it
            ).ToHashSet();


        private HashSet<ApplicationCommand> GetRegisteredCommandsOfType(ApplicationCommandType type)
            => AllRegisteredCommands.Where(x => x.CommandType == type).ToHashSet();

        public HashSet<ApplicationCommand> RegisteredSlashCommands => GetRegisteredCommandsOfType(Slash);
        public HashSet<ApplicationCommand> RegisteredUserCommands => GetRegisteredCommandsOfType(User);
        public HashSet<ApplicationCommand> RegisteredMessageCommands => GetRegisteredCommandsOfType(Message);

        public void RegisterCommands(params ApplicationCommand[] commands) =>
            commands.WhereNotNull().ForEach(c => AllRegisteredCommands.Add(c));

        public void DeregisterCommands(params ApplicationCommand[] commands) =>
            commands.WhereNotNull().ForEach(c => AllRegisteredCommands.Remove(c));

        private ApplicationCommand GetCommand(string name)
            => AllRegisteredCommands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
    }
}