using Discord.Interactions;

namespace Volte.Interactions.Commands.Modules;

public partial class InteractionSettingsModule
{
    [SlashCommand("notify-acc-age", "Enables/disable warnings when a user who joins has an account created within the last month.")]
    public Task<RuntimeResult> VerifyAgeAsync(bool enabled)
    {
        ModifyData(data => data.Configuration.Moderation.CheckAccountAge = enabled);
        return Ok(enabled ? "Account age detection has been enabled." : "Account age detection has been disabled.");
    }
}