using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("embed-tags", "Toggles whether or not tags requested in your guild will be in an embed.")]
    public Task<RuntimeResult> EmbedTagsAsync(bool enabled)
    {
        ModifyData(data => data.Settings.EmbedTags = enabled);
        return Ok(enabled
            ? "Tags will now show their requester and be displayed in an embed!"
            : "Tags will **NO LONGER** show their requester and be displayed in an embed!", ephemeral: true);
    }
}