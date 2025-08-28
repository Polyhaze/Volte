namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class SettingsModule
{
    [Command("ModRole", "Mod")]
    [Description("Sets the role able to use Moderation commands for the current guild.")]
    public Task<ActionResult> ModRoleAsync(
        [Remainder,
         Description("The role to be set as the Moderator role; or none if you want to see the current one.")]
        SocketRole role = null)
    {
        if (role is null)
            return Ok(
                $"The current Moderator role in this guild is <@&{Context.GuildData.Settings.Moderation.ModRole}>.");

        Context.Modify(data => data.Settings.Moderation.ModRole = role.Id);
        return Ok($"Set {role.Mention} as the Moderator role for this guild.");
    }
    
    [Command("SecondaryModRole", "ModRole2", "Mod2")]
    [Description("Sets a secondary role able to use Moderation commands for the current guild.")]
    public Task<ActionResult> SecondaryModRoleAsync(
        [Remainder,
         Description("The role to be set as the secondary Moderator role; or none if you want to reset.")]
        SocketRole role = null)
    {
        if (role is null)
        {
            Context.Modify(data => data.Settings.Moderation.SecondaryModRole = 0);
            return Ok("Reset the secondary Moderator role.");
        }

        Context.Modify(data => data.Settings.Moderation.ModRole = role.Id);
        return Ok($"Set {role.Mention} as the secondary Moderator role for this guild.");
    }
}