using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionUtilityModule
{
    [SlashCommand("iam", "Gives yourself a role, if it is in the current guild's self role list.")]
    public async Task<RuntimeResult> IamAsync(
        [Summary(description: "The Self Role you want to give yourself.")]
        SocketRole role
    )
    {
        if (!IsInGuild())
            return BadRequest("You can only use this command in guilds.");
        
        var data = GetData();
        if (data.Extras.SelfRoles.Count == 0)
            return BadRequest("This guild does not have any roles you can give yourself.");

        if (!data.Extras.SelfRoles.Contains(role.Id))
            return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");

        await Context.Guild.GetUser(Context.User.Id).AddRoleAsync(role);
        return Ok($"Gave you the **{role.Name}** role.", true);
    }
    
    [SlashCommand("iamnot", "Take a role from yourself, if it is in the current guild's self role list.")]
    public async Task<RuntimeResult> IamNotAsync(
        [Summary(description: "The Self Role you want to remove from yourself.")]
        SocketRole role
    )
    {
        if (!IsInGuild())
            return BadRequest("You can only use this command in guilds.");
        
        var data = GetData();
        if (!data.Extras.SelfRoles.Contains(role.Id))
            return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");

        await Context.Guild.GetUser(Context.User.Id).RemoveRoleAsync(role.Id);
        return Ok($"Took away your **{role.Name}** role.", true);
    }
}