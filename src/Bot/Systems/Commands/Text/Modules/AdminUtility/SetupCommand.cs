namespace Volte.Systems.Commands.Text.Modules;

public partial class AdminUtilityModule
{
    [Command("Setup")]
    [Description("Interactively setup your guild's moderator and admin roles.")]
    public Task<ActionResult> SetupAsync() 
        => Ok(async () =>
        {
            adminRole:
            await Context.CreateEmbed("What role would you like to have Admin permission with me?")
                .SendToAsync(Context.Channel);
            var (role, didTimeout, message) = await Context.GetNextAsync<SocketRole>();
            if (didTimeout) return;
            if (!role.HasValue) goto adminRole;
                
            Context.Modify(data => data.Configuration.Moderation.AdminRole = role.Value.Id);
            
            await message.AddReactionAsync(Emojis.BallotBoxWithCheck);

            modRole:
            await Context.CreateEmbed("What role would you like to have Moderator permission with me?")
                .SendToAsync(Context.Channel);
                
            (role, didTimeout, message) = await Context.GetNextAsync<SocketRole>();
            if (didTimeout) return;
            if (!role.HasValue) goto modRole;

            Context.Modify(data => data.Configuration.Moderation.ModRole = role.Value.Id);

            await message.AddReactionAsync(Emojis.BallotBoxWithCheck);
            
            await Context.CreateEmbed("Done. People with those roles can now use their respective commands.")
                .SendToAsync(Context.Channel);

        }, false);
        
}