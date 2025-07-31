namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("IamNot")]
    [Description("Take a role from yourself, if it is in the current guild's self role list.")]
    public async Task<ActionResult> IamNotAsync(
        [Remainder, Description("The Self Role you want to remove from yourself.")]
        SocketRole role)
    {
        if (!Context.GuildData.Settings.Collections.SelfRoles.Contains(role.Id))
            return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");

        await Context.User.RemoveRoleAsync(role.Id);
        return Ok($"Took away your **{role.Name}** role.");
    }
}