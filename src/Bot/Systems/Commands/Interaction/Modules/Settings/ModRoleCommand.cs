using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("mod-role", "Sets the role able to use Moderation commands for the current guild.")]
    public Task<RuntimeResult> ModRoleAsync(
        [Summary(description: "The role to be set as the Moderator role; or none if you want to see the current one.")]
        SocketRole role = null)
    {
        if (role is null)
            return Ok($"The current Moderator role for this guild is <@&{GetData().Configuration.Moderation.ModRole}>", ephemeral: true);

        ModifyData(data => data.Configuration.Moderation.ModRole = role.Id);
        return Ok($"Set {role.Mention} as the Moderator role for this guild.", ephemeral: true);
    }
}