using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("auto-role", "Sets the role to be used for Autorole.")]
    public Task<RuntimeResult> AutoroleAsync(
        [Summary(description: "The role to be given to users when they join; or none to see the current one.")] 
        SocketRole role = null)
    {
        if (role is null)
            return Ok($"The current Autorole for this guild is <@&{GetData().Settings.Autorole}>", ephemeral: true);

        ModifyData(data => data.Settings.Autorole = role.Id);
        return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.", ephemeral: true);
    }
}