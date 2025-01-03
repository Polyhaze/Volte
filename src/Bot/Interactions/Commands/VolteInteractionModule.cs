using Discord.Interactions;
using Volte.Interactions.Results;

namespace Volte.Interactions.Commands;

public abstract class VolteInteractionModule<T> : InteractionModuleBase<SocketInteractionContext<T>> where T : SocketInteraction
{
    public bool IsInGuild() => Context.Guild != null;

    public GuildData GetData() 
        => VolteBot.Services.Get<DatabaseService>().GetData(Context.Guild);
    
    public void ModifyData(DataEditor modifier)
        => VolteBot.Services.Get<DatabaseService>().Modify(Context.Guild.Id, modifier);

    protected InteractionBadRequestResult BadRequest(string reason) => new(reason);

    protected InteractionOkResult<T> Ok(ReplyBuilder<T> reply) => new(reply);

    protected InteractionOkResult<T> Ok(string message, bool ephemeral = false) 
        => Ok(Context.CreateReplyBuilder(ephemeral).WithEmbedFrom(message));
    
    protected InteractionOkResult<T> Ok(EmbedBuilder embed, bool ephemeral = false) 
        => new(Context.CreateReplyBuilder(ephemeral).WithEmbeds(embed));
}

public abstract class VolteSlashCommandModule : VolteInteractionModule<SocketSlashCommand>;
public abstract class VolteMessageCommandModule : VolteInteractionModule<SocketMessageCommand>;
public abstract class VolteUserCommandModule : VolteInteractionModule<SocketUserCommand>;
public abstract class VolteMessageComponentModule : VolteInteractionModule<SocketMessageComponent>;
public abstract class VolteModalModule : VolteInteractionModule<SocketModal>;