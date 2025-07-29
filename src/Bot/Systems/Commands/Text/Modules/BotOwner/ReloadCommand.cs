namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class BotOwnerModule
{
    [Command("Reload", "Rl")]
    [Description(
        "Repopulates AllowedPasteSites and reloads the bot's configuration file if you've changed it. Note: if the file's content is invalid JSON things might start acting up.")]
    public async Task<ActionResult> ReloadAsync()
    {
        AdminUtilityModule.AllowedPasteSites = await HttpHelper.GetAllowedPasteSitesAsync(Context.Services);
        return Config.Reload<HeadlessBotConfig>()
            ? Ok("Config and AllowedPasteSites reloaded!")
            : BadRequest("Something bad happened. Check console for more detailed information.");
    }
}