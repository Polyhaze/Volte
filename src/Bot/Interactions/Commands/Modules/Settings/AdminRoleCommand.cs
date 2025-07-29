using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("admin-role", "Sets the role able to use Admin commands for the current guild.")]
    public Task<RuntimeResult> AdminRoleAsync(
        [Summary(description: "The role to be set as the Admin role; or none if you want to see the current one.")]
        SocketRole role = null)
    {
        if (role is null)
            return Ok($"The current Admin role for this guild is <@&{GetData().Configuration.Moderation.AdminRole}>", ephemeral: true);

        ModifyData(data => data.Configuration.Moderation.AdminRole = role.Id);
        return Ok($"Set {role.Mention} as the Admin role for this guild.", ephemeral: true);
    }
}