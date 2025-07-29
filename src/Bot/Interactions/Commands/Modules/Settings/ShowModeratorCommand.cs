using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("show-mod", "Enables/Disables showing the moderator responsible for punishing users.")]
    public Task<RuntimeResult> ShowModeratorAsync(bool enabled)
    {
        ModifyData(data => data.Configuration.Moderation.ShowResponsibleModerator = enabled);
        return Ok(enabled
            ? "Enabled showing the responsible moderator to users when they're punished."
            : "Disabled showing the responsible moderator to users when they're punished.", ephemeral: true);
    }
}