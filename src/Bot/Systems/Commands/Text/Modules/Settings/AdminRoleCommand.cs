namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class SettingsModule
{
    [Command("AdminRole", "Admin")]
    [Description("Sets the role able to use Admin commands for the current guild.")]
    public Task<ActionResult> AdminRoleAsync([Remainder, Description("The role to be set as the Admin role; or none if you want to see the current one.")] SocketRole role = null)
    {
        if (role is null)
            return Ok($"The current Admin role in this guild is <@&{Context.GuildData.Settings.Moderation.AdminRole}>.");
            
        Context.Modify(data => data.Settings.Moderation.AdminRole = role.Id);
        return Ok($"Set {role.Mention} as the Admin role for this guild.");
    }
}