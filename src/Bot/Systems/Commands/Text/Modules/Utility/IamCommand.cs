namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Iam")]
    [Description("Gives yourself a role, if it is in the current guild's self role list.")]
    public async Task<ActionResult> IamAsync([Remainder, Description("The Self Role you want to give yourself.")]
        SocketRole role)
    {
        if (Context.GuildData.Settings.Collections.SelfRoles.Count == 0)
            return BadRequest("This guild does not have any roles you can give yourself.");

        if (!Context.GuildData.Settings.Collections.SelfRoles.Contains(role.Id))
            return BadRequest($"The role **{role.Name}** isn't in the self roles list for this guild.");

        await Context.User.AddRoleAsync(role);
        return Ok($"Gave you the **{role.Name}** role.");
    }
}