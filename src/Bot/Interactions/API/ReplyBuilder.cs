using Discord.Interactions;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Interactions;

/// <summary>
///     Acts as a mechanism to easily construct responses to Discord Interactions.<br/><br/>
///     The "build" functions on this builder are the Async-suffixed methods:
///     <see cref="RespondAsync"/>, <see cref="ModifyOriginalResponseAsync"/>, <see cref="FollowupAsync"/>
///     <br/><br/>
///     In combination with the command results system, this also has smart deferral;
///     that is, if you defer in a command (using the Module's
///     <see cref="Volte.Interactions.Commands.VolteInteractionModule{TInteraction}.DeferAsync"/> implementation),
///     the ReplyBuilder will know to modify the original response instead of
///     trying to make a new response.<br/>
///
///     The ReplyBuilder does not magically do this on its own; the custom DeferAsync effectively
///     just sets <see cref="DidDefer"/> in this class to true when it's constructed;
///     and then that value is respected when <see cref="ExecuteAsync"/> is called.
/// </summary>
/// <typeparam name="TInteraction">The type of Interaction that's being responded to.</typeparam>
#nullable enable
public class ReplyBuilder<TInteraction> where TInteraction : SocketInteraction
{
    public SocketInteractionContext<TInteraction> Context { get; }

    public string? Content { get; private set; }
    public HashSet<Embed> Embeds { get; } = [];
    public bool IsTts { get; private set; }
    public bool IsEphemeral { get; private set; }
    public bool ShouldFollowup { get; private set; }
    public bool DidDefer { get; private set; }
    public AllowedMentions AllowedMentions { get; private set; } = AllowedMentions.None;
    public Task UpdateOrNoopTask => _updateTask ?? Task.CompletedTask;
    private Task? _updateTask;
    public HashSet<ActionRowBuilder> ActionRows { get; } = [];

    public ReplyBuilder(SocketInteractionContext<TInteraction> ctx) => Context = ctx;

    public ReplyBuilder<TInteraction> WithContent(string content)
    {
        Content = content;
        return this;
    }

    public ReplyBuilder<TInteraction> WithEmbed(Action<EmbedBuilder> modifier)
    {
        Embeds.Add(Context.CreateEmbedBuilder().Apply(modifier).Build());
        return this;
    }

    public ReplyBuilder<TInteraction> WithEmbeds(IEnumerable<EmbedBuilder> embedBuilders)
    {
        embedBuilders.ForEach(x => Embeds.Add(x.Build()));
        return this;
    }

    public ReplyBuilder<TInteraction> WithEmbeds(IEnumerable<Embed> embeds)
    {
        embeds.ForEach(x => Embeds.Add(x));
        return this;
    }

    public ReplyBuilder<TInteraction> WithEmbeds(params EmbedBuilder[] embedBuilders) =>
        WithEmbeds(embedBuilders.Select(static x => x.Build()));

    public ReplyBuilder<TInteraction> WithEmbeds(params Embed[] embeds) => WithEmbeds(embeds.ToList());

    public ReplyBuilder<TInteraction> WithEmbedFrom(StringBuilder content)
        => WithEmbedFrom(content.ToString());

    public ReplyBuilder<TInteraction> WithEmbedFrom(string content)
    {
        Embeds.Add(Context.CreateEmbedBuilder(content).Build());
        return this;
    }

    public ReplyBuilder<TInteraction> WithTts(bool tts)
    {
        IsTts = tts;
        return this;
    }

    public ReplyBuilder<TInteraction> WithEphemeral(bool ephemeral = true)
    {
        IsEphemeral = ephemeral;
        return this;
    }
    
    public ReplyBuilder<TInteraction> WithDeferral(bool deferred = true)
    {
        DidDefer = deferred;
        return this;
    }
    
    public ReplyBuilder<TInteraction> WithAutoFollowup(bool followup = true)
    {
        ShouldFollowup = followup;
        return this;
    }

    public ReplyBuilder<TInteraction> WithAllowedMentions(AllowedMentions allowedMentions)
    {
        AllowedMentions = allowedMentions;
        return this;
    }

    public ReplyBuilder<TInteraction> WithComponentMessageUpdate(Action<MessageProperties> modifier,
        RequestOptions? options = null)
    {
        if (Context.Interaction is SocketMessageComponent smc)
            _updateTask = smc.UpdateAsync(modifier, options);

        return this;
    }

    public ReplyBuilder<TInteraction> WithComponent(ComponentBuilder builder) 
        => WithActionRows(builder.ActionRows);

    public ReplyBuilder<TInteraction> WithActionRows(params ActionRowBuilder[] actionRows)
    {
        actionRows.ForEach(row => ActionRows.Add(row));
        return this;
    }

    public ReplyBuilder<TInteraction> WithActionRows(IEnumerable<ActionRowBuilder> actionRows)
        => WithActionRows(actionRows.ToArray());

    public ReplyBuilder<TInteraction> WithButtons(IEnumerable<ButtonBuilder> buttons)
        => WithButtons(buttons.ToArray());

    public ReplyBuilder<TInteraction> WithButtons(params ButtonBuilder[] buttons) 
        => WithActionRows(buttons.AsActionRow());


    public ReplyBuilder<TInteraction> WithSelectMenu(SelectMenuBuilder menu)
    {
        ActionRows.Add(new ActionRowBuilder().AddComponent(menu));
        return this;
    }

    public Task ExecuteAsync(RequestOptions? options = null) =>
        DidDefer
            ? ModifyOriginalResponseAsync(options)
            : ShouldFollowup
                ? FollowupAsync(options)
                : RespondAsync(options);


    public Task<RestInteractionMessage> ModifyOriginalResponseAsync(RequestOptions? options = null)
    {
        return Context.Interaction.ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = opt(Content);
            msg.AllowedMentions = opt(AllowedMentions);
            msg.Embeds = opt(Embeds.ToArray());
            if (ActionRows.Count > 0)
                msg.Components = opt(new ComponentBuilder().AddActionRows(ActionRows).Build());
        }, options)
            .ThenApply(_ => UpdateOrNoopTask);

        Discord.Optional<T> opt<T>(T value)
        {
            return value is null 
                ? Discord.Optional<T>.Unspecified 
                : new Discord.Optional<T>(value);
        }
    }

    public Task RespondAsync(RequestOptions? options = null)
        => Context.Interaction.RespondAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, new ComponentBuilder().AddActionRows(ActionRows).Build(), options: options)
            .Then(() => UpdateOrNoopTask);


    public Task<RestFollowupMessage> FollowupAsync(RequestOptions? options = null)
        => Context.Interaction.FollowupAsync(Content, Embeds.ToArray(), IsTts, IsEphemeral,
                AllowedMentions, new ComponentBuilder().AddActionRows(ActionRows).Build(), options: options)
            .ThenApply(_ => UpdateOrNoopTask);
}