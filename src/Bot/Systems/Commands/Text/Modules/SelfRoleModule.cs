namespace Volte.Systems.Commands.Text.Modules;

[Group("SelfRole", "Sr")]
public class SelfRoleModule : VolteModule
{
    [Command, DummyCommand, Description("The command group for modifying SelfRoles.")]
    public async Task<ActionResult> BaseAsync() =>
        Ok(await TextCommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

    [Command("List", "Ls", "L")]
    [Description("Gets a list of self roles available for this guild.")]
    public Task<ActionResult> SelfRoleListAsync()
    {
        if (Context.GuildData.Settings.Collections.SelfRoles.None())
            return BadRequest("No roles available to self-assign in this guild.");

        var roles = Context.GuildData.Settings.Collections.SelfRoles.Select(x =>
            Context.Guild.Roles.TryGetFirst(r => r.Id == x, out var role)
                ? Format.Bold(role.Name)
                : string.Empty
        ).Where(static x => !x.IsNullOrEmpty())
            .JoinToString("\n");

        return Ok(Context.CreateEmbedBuilder(roles).WithTitle("Roles available to self-assign in this guild:"));
    }

    [Command("Add", "A")]
    [Description("Adds a role to the list of self roles for this guild.")]
    [RequireGuildAdmin]
    public Task<ActionResult> SelfRoleAddAsync(
        [Remainder, Description("The role to add to the SelfRoles list.")] SocketRole role)
    {
        var target = Context.Guild.GetRole(Context.GuildData.Settings.Collections.SelfRoles.FirstOrDefault(x => x == role.Id));
        if (target is not null)
            return BadRequest(
                $"A role with the name **{role.Name}** is already in the Self Roles list for this guild!");

        Context.Modify(data => data.Settings.Collections.SelfRoles.Add(role.Id));
        return Ok($"Successfully added **{role.Name}** to the Self Roles list for this guild.");
    }

    [Command("Remove", "Rem", "R")]
    [Description("Removes a role from the list of self roles for this guild.")]
    [RequireGuildAdmin]
    public Task<ActionResult> SelfRoleRemoveAsync(
        [Remainder, Description("The role to remove from the SelfRoles list.")]
        SocketRole role)
    {
        if (!Context.GuildData.Settings.Collections.SelfRoles.Contains(role.Id))
            return BadRequest($"The Self Roles list for this guild doesn't contain **{role.Name}**.");

        Context.Modify(data => data.Settings.Collections.SelfRoles.Remove(role.Id));
        Db.Save(Context.GuildData);
        return Ok($"Removed **{role.Name}** from the Self Roles list for this guild.");
    }

    [Command("Clear", "Cl", "C")]
    [Description("Clears the self role list for this guild.")]
    [RequireGuildAdmin]
    public Task<ActionResult> SelfRoleClearAsync()
    {
        Context.Modify(data => data.Settings.Collections.SelfRoles.Clear());
        return Ok("Successfully cleared all Self Roles for this guild.");
    }
}