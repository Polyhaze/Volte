using Discord.Interactions;
using Volte.Systems.Commands.Interaction;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Entities;

public class ModActionEventArgs
{
    public SocketUser Moderator { get; private set; }
    public ISocketMessageChannel Channel { get; private set; }
    public required Func<string, EmbedBuilder> CreateEmbedBuilder { get; init; }
    public required GuildDataV2 GuildData { get; init; }
    
    public ModActionType ActionType { get; private set; }
    public string Reason { get; private set; }
    public ulong? TargetId { get; private set; }
    public IUser TargetUser { get; private set; }
    public int? Count { get; private set; }
    public DateTimeOffset Time { get; private set; }
    public SocketGuild Guild { get; private set; }

    public static ModActionEventArgs InContext(VolteContext ctx) => new ModActionEventArgs
    {
        CreateEmbedBuilder = ctx.CreateEmbedBuilder,
        GuildData = ctx.GuildData,
        Channel = ctx.Channel
    }.WithDefaultsFromContext(ctx);
    
#pragma warning disable CS0618 // Type or member is obsolete
    public static ModActionEventArgs FromModule<TInteraction>(VolteInteractionModule<TInteraction> mdl) 
#pragma warning restore CS0618 // Type or member is obsolete
        where TInteraction : SocketInteraction => new ModActionEventArgs
    {
        CreateEmbedBuilder = mdl.Context.CreateEmbedBuilder,
        GuildData = mdl.GetData(),
        Channel = mdl.Context.Channel
    }.WithDefaultsFromContext(mdl.Context);

    public ModActionEventArgs WithModerator(SocketUser user)
    {
        Moderator = user;
        return this;
    }

    public ModActionEventArgs WithActionType(ModActionType type)
    {
        ActionType = type;
        return this;
    }

    public ModActionEventArgs WithReason(string reason)
    {
        Reason = reason;
        return this;
    }

    public ModActionEventArgs WithTarget(ulong? id)
    {
        TargetId = id;
        return this;
    }

    public ModActionEventArgs WithTarget(IUser user)
    {
        TargetUser = user;
        return this;
    }

    public ModActionEventArgs WithCount(int? count)
    {
        Count = count;
        return this;
    }

    public ModActionEventArgs WithTime(DateTimeOffset time)
    {
        Time = time;
        return this;
    }

    public ModActionEventArgs WithGuild(SocketGuild guild)
    {
        Guild = guild;
        return this;
    }

    public ModActionEventArgs WithDefaultsFromContext(VolteContext ctx)
    {
        return WithTime(ctx.Now)
            .WithGuild(ctx.Guild)
            .WithModerator(ctx.User);
    }
    
    public ModActionEventArgs WithDefaultsFromContext<TInteraction>(SocketInteractionContext<TInteraction> ctx) 
        where TInteraction : SocketInteraction
    {
        return WithTime(ctx.Interaction.CreatedAt)
            .WithGuild(ctx.Guild)
            .WithModerator(ctx.User);
    }

    public ModerationService.ModLogMessageBuilder MessageBuilder() => new(this);
}