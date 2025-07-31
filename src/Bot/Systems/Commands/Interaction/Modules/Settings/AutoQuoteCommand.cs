using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("auto-quote", "Enables or disables the message URL quoting for this guild.")]
    public Task<RuntimeResult> AutoQuoteAsync(bool enabled)
    {
        ModifyData(data => data.Settings.AutoQuoteMessageUrls = enabled);
        return Ok(enabled ? "Enabled auto quotes for this guild." : "Disabled auto quotes for this guild.", ephemeral: true);
    }
}