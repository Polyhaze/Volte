using Starscript;
using Volte.Systems.Database;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.Interactive;
using Volte.Systems.Starscript;

namespace Volte.Systems.Commands.Text;

public sealed class VolteContext : CommandContext, IStarscriptObject
{
    // ReSharper disable once SuggestBaseTypeForParameter
    public VolteContext(SocketMessage msg, IServiceProvider provider) : base(provider)
    {
        Client = provider.Get<DiscordSocketClient>();
        Guild = msg.Channel.Cast<SocketTextChannel>()?.Guild;
        Interactive = provider.Get<InteractiveService>();
        Channel = msg.Channel.Cast<SocketTextChannel>();
        User = msg.Author.Cast<SocketGuildUser>();
        Message = msg.Cast<SocketUserMessage>();
        GuildData = provider.Get<DatabaseService>().GetData(Guild);
        Now = DateTime.Now;
    }


    public DiscordSocketClient Client { get; }
    public SocketGuild Guild { get; }
    public InteractiveService Interactive { get; }
    public SocketTextChannel Channel { get; }
    public SocketGuildUser User { get; }
    public SocketUserMessage Message { get; }
    public GuildDataV2 GuildData { get; }
    public DateTime Now { get; }
        
    public Embed CreateEmbed(StringBuilder content) => CreateEmbed(content.ToString());

    public Embed CreateEmbed(Action<EmbedBuilder> action) 
        => CreateEmbedBuilder().Apply(action).Build();
        
    public Embed CreateEmbed(string content) => CreateEmbedBuilder(content).Build();
    
    public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
        .WithColor(User.GetHighestRole()?.Color ?? new Color(Config.SuccessColor))
        .WithAuthor(User.ToString(), User.GetEffectiveAvatarUrl())
        .WithDescription(content ?? string.Empty);

    public EmbedBuilder CreateEmbedBuilder(StringBuilder content) => CreateEmbedBuilder(content.ToString());
        
    /// <summary>
    ///     Waits for a message containing content parseable by a registered <see cref="TypeParser{T}"/>.
    ///     Waiting times out after <paramref name="timeout"/> is over; returning no result and DidTimeout as true;
    ///     Receiving a message results in this method parsing its contents via the <see cref="TypeParser{T}"/>.
    ///     To disable a timeout, set it to <see cref="Timeout"/>.<see cref="Timeout.InfiniteTimeSpan"/>
    ///     TL;DR: Your <see cref="CommandService"/> must have a <see cref="TypeParser{T}"/> for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="timeout">The timespan to wait for. Defaults to 30 seconds.</param>
    /// <typeparam name="T">The type of object to wait for.</typeparam>
    public async ValueTask<(Gommon.Optional<T> Result, bool DidTimeout, SocketUserMessage Message)> GetNextAsync<T>(TimeSpan? timeout = null)
    {
        timeout ??= 30.Seconds();
        var message = await Interactive.NextMessageAsync(this, timeout: timeout);
        if (message is null)
        {
            await CreateEmbed($"You didn't reply within {timeout.Value.Humanize()}. Run the command and try again.")
                .SendToAsync(Channel);
            return (default, true, null);
        }

        var parserResult = await Services.Get<CommandService>().GetTypeParser<T>().ParseAsync(null, message.Content, this);
        return parserResult.IsSuccessful 
            ? (parserResult.Value, false, message) 
            : (default, false, message);
    }
    
    /// <summary>
    ///     Waits for a message.
    ///     Waiting times out after <paramref name="timeout"/> is over; returning no result and DidTimeout as true.
    ///     To disable a timeout, set it to <see cref="Timeout"/>.<see cref="Timeout.InfiniteTimeSpan"/>.
    /// </summary>
    /// <param name="timeout">The timespan to wait for. Defaults to 30 seconds.</param>
    public async ValueTask<(Gommon.Optional<SocketUserMessage> Result, bool DidTimeout)> GetNextAsync(TimeSpan? timeout = null)
    {
        timeout ??= 30.Seconds();
        var message = await Interactive.NextMessageAsync(this, timeout: timeout);
        if (message is null)
        {
            await CreateEmbed($"You didn't reply within {timeout.Value.Humanize()}. Run the command and try again.")
                .SendToAsync(Channel);
            return (default, true);
        }

        return (message, false);
    }
    
    /// <summary>
    ///     Waits for a message containing content parseable by <see cref="Enum.TryParse{T}(string, bool, out T)"/>.
    ///     Waiting times out after <paramref name="timeout"/> is over; returning no result and DidTimeout as true;
    ///     Receiving a message results in this method parsing its contents via <see cref="Enum.TryParse{T}(string, bool, out T)"/>.
    ///     To disable a timeout, set it to <see cref="Timeout"/>.<see cref="Timeout.InfiniteTimeSpan"/>.
    /// </summary>
    /// <param name="timeout">The timespan to wait for. Defaults to 30 seconds.</param>
    /// <typeparam name="T">The type of enum to wait for.</typeparam>
    public async ValueTask<(Gommon.Optional<T> Result, bool DidTimeout, SocketUserMessage Message)> GetNextEnumAsync<T>(TimeSpan? timeout = null) where T : struct 
    {
        timeout ??= 30.Seconds();
        var message = await Interactive.NextMessageAsync(this, timeout: timeout);
        if (message is null)
        {
            await CreateEmbed($"You didn't reply within {timeout.Value.Humanize()}. Run the command and try again.")
                .SendToAsync(Channel);
            return (default, true, null);
        }

        if (Enum.TryParse<T>(message.Content, ignoreCase: true, out var result))
            return (result, false, message);

        return (default, false, message);
    }

    public string FormatUsageFor(string commandName) => 
        TextCommandHelper.FormatUsage(this, Services.Get<CommandService>().GetCommand(commandName));
        

    public void Modify(DataEditor modifier)
    {
        modifier(GuildData);
        if (Services.TryGet<DatabaseService>(out var db))
            db.Save(GuildData);
    }

    public StringSegment RunStarscript(string source) => VolteStarscript.Run(source, this);
    public StringSegment RunStarscriptExpression(string expression) 
        => RunStarscript($"{{{expression.Replace("{", string.Empty).Replace("}", string.Empty)}}}");

    public ValueMap ToStarscript() => new ValueMap()
        .Set("message", StarscriptHelper.Wrap(Message))
        .Set("guild", StarscriptHelper.Wrap(Guild))
        .Set("user", StarscriptHelper.Wrap(User));
}