using Discord.Interactions;
using Volte.Interactions.Results;
using IResult = Discord.Interactions.IResult;

namespace Volte.Interactions;

public class VolteInteractionService : VolteService
{
    private static bool _commandsRegistered;
    
    private readonly IServiceProvider _provider;
    private readonly InteractionService _backing;

    public VolteInteractionService(IServiceProvider provider, DiscordSocketClient client)
    {
        _provider = provider;
        _backing = new(client.Rest, new()
        {
            LogLevel = Config.DebugEnabled || Version.IsDevelopment
                ? LogSeverity.Debug
                : LogSeverity.Verbose,
            InteractionCustomIdDelimiters = [MessageComponentId.Separator]
        });

        {
            client.SlashCommandExecuted += async interaction =>
            {
                var ctx = new SocketInteractionContext<SocketSlashCommand>(client, interaction);
                await _backing.ExecuteCommandAsync(ctx, provider);
            };

            client.MessageCommandExecuted += async interaction =>
            {
                var ctx = new SocketInteractionContext<SocketMessageCommand>(client, interaction);
                await _backing.ExecuteCommandAsync(ctx, provider);
            };

            client.UserCommandExecuted += async interaction =>
            {
                var ctx = new SocketInteractionContext<SocketUserCommand>(client, interaction);
                await _backing.ExecuteCommandAsync(ctx, provider);
            };
            
            client.AutocompleteExecuted += async interaction =>
            {
                var ctx = new SocketInteractionContext<SocketAutocompleteInteraction>(client, interaction);
                await _backing.ExecuteCommandAsync(ctx, provider);
            };
        }

        _backing.Log += logMessage =>
        {
            Log(new VolteLogEventArgs(logMessage));
            return Task.CompletedTask;
        };

        _backing.SlashCommandExecuted += (info, context, result) => 
            OnSlashCommandExecuted(info, context.Cast<SocketInteractionContext<SocketSlashCommand>>(), result);
        _backing.ContextCommandExecuted += async (info, context, result) =>
        {
            if (context.Interaction is SocketMessageCommand)
                await OnContextCommandExecuted(info, context.Cast<SocketInteractionContext<SocketMessageCommand>>(),
                    result);
            else
                await OnContextCommandExecuted(info, context.Cast<SocketInteractionContext<SocketUserCommand>>(),
                    result);
        };
    }

    public async Task InitAsync()
    {
        if (!_commandsRegistered)
        {
            await _backing.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);

#if DEBUG
            await _backing.RegisterCommandsToGuildAsync(DiscordHelper.DevGuildId);
#else
            await _backing.RegisterCommandsGloballyAsync();
#endif
            _commandsRegistered = true;
        }
    }

    #region Event Handlers

    private static Task OnSlashCommandExecuted(
        SlashCommandInfo commandInfo,
        SocketInteractionContext<SocketSlashCommand> context, 
        IResult result
    ) => OnCommandExecuted<SocketSlashCommand, SlashCommandInfo, SlashCommandParameterInfo>(commandInfo, context, result);

    private static Task OnContextCommandExecuted<TInteraction>(
        ContextCommandInfo commandInfo,
        SocketInteractionContext<TInteraction> context,
        IResult result
    ) where TInteraction : SocketInteraction
        => OnCommandExecuted<TInteraction, ContextCommandInfo, CommandParameterInfo>(commandInfo, context, result);

    private static async Task OnCommandExecuted<TInteraction, TCommandInfo, TParameterInfo>(TCommandInfo _,
        SocketInteractionContext<TInteraction> context, IResult result)
        where TInteraction : SocketInteraction 
        where TParameterInfo : CommandParameterInfo
        where TCommandInfo : CommandInfo<TParameterInfo>
    {
        switch (result)
        {
            case InteractionResultBase okResult:
                await okResult.ExecuteAsync();
                break;
            case PreconditionResult unmetPreconditionResult:
                await context.CreateReplyBuilder(true)
                    .WithEmbed(e =>
                        e.WithTitle(unmetPreconditionResult.ErrorReason)
                            .WithColor(Color.DarkRed)
                            .WithCurrentTimestamp()
                    ).RespondAsync();
                break;
        }
    }

    #endregion Event Handlers
}