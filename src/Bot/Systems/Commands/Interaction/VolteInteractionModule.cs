using Discord.Interactions;
using Volte.Systems.Database;
using Volte.Systems.Database.Entities;
using Volte.Systems.Database.EntitiesV2;
using Volte.Systems.Interactions;

namespace Volte.Systems.Commands.Interaction;

[Obsolete("Use an inheritor of this class; not this class directly.")]
public abstract class VolteInteractionModule<T> : InteractionModuleBase<SocketInteractionContext<T>> where T : SocketInteraction
{
    public DatabaseService Db { get; set; }
    
    private bool DidDefer { get; set; }

    protected new Task DeferAsync(bool ephemeral = false, RequestOptions options = null)
    {
        if (DidDefer)
            return Task.CompletedTask;

        DidDefer = true;
        
        return base.DeferAsync(ephemeral, options);
    }

    protected ReplyBuilder<T> CreateReplyBuilder(
        bool ephemeral = false
    ) => Context.CreateReplyBuilder(ephemeral, DidDefer);
    
    public bool IsInGuild() => Context.Guild != null;

    public GuildDataV2 GetData() => Db.GetData(Context.Guild);
    
    public void ModifyData(DataEditor modifier) => Db.Modify(Context.Guild.Id, modifier);

    protected InteractionNoneResult<T> None() => new(Context, DidDefer);
    
    protected InteractionBadRequestResult<T> BadRequest(string reason) => new(Context, reason, DidDefer);

    protected InteractionOkResult<T> Ok(ReplyBuilder<T> reply) => new(reply);
    
    protected InteractionOkResult<T> Ok(ReplyBuilder<T> reply, AsyncFunction onComplete) => new(reply) { AfterCompletion = onComplete };

    protected InteractionOkResult<T> Ok(string message, bool ephemeral = false) 
        => Ok(CreateReplyBuilder(ephemeral).WithEmbedFrom(message));
    
    protected InteractionOkResult<T> Ok(StringBuilder message, bool ephemeral = false) 
        => Ok(CreateReplyBuilder(ephemeral).WithEmbedFrom(message));
    
    protected InteractionOkResult<T> Ok(string message, AsyncFunction onComplete, bool ephemeral = false) 
        => Ok(CreateReplyBuilder(ephemeral).WithEmbedFrom(message), onComplete);
    
    protected InteractionOkResult<T> Ok(StringBuilder message, AsyncFunction onComplete, bool ephemeral = false) 
        => Ok(CreateReplyBuilder(ephemeral).WithEmbedFrom(message), onComplete);
    
    protected InteractionOkResult<T> Ok(EmbedBuilder embed, bool ephemeral = false) 
        => new(CreateReplyBuilder(ephemeral).WithEmbeds(embed));
    
    protected InteractionOkResult<T> Ok(EmbedBuilder embed, AsyncFunction onComplete, bool ephemeral = false) 
        => new(CreateReplyBuilder(ephemeral).WithEmbeds(embed)) { AfterCompletion = onComplete };
}

#pragma warning disable CS0618 // Type or member is obsolete
public abstract class VolteSlashCommandModule : VolteInteractionModule<SocketSlashCommand>;
public abstract class VolteMessageCommandModule : VolteInteractionModule<SocketMessageCommand>;
public abstract class VolteUserCommandModule : VolteInteractionModule<SocketUserCommand>;
public abstract class VolteMessageComponentModule : VolteInteractionModule<SocketMessageComponent>;
public abstract class VolteModalModule : VolteInteractionModule<SocketModal>;
#pragma warning restore CS0618 // Type or member is obsolete