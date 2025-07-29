namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class AdminUtilityModule
{
    [Command("AddRole", "Ar")]
    [Description("Grants a role to the mentioned member.")]
    public async Task<ActionResult> AddRoleAsync([Description("The member to add the role to.")]
        SocketGuildUser member, [Remainder, Description("The role to give the member.")]
        SocketRole role)
    {
        if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            return BadRequest("Role position is too high for me to be able to grant it to anyone.");

        await member.AddRoleAsync(role);
        return Ok($"Added the role **{role}** to {member.Mention}!");
    }
}