namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("IsBanned")]
    [Description("Check if a user is banned. Useful for large servers.")]
    public async Task<ActionResult> IsBannedAsync([Description("The user to check.")] RestUser user)
    {
        if (await Context.Guild.GetBanAsync(user) is { } ban)
            return Ok($"Yes; {Format.Bold(Format.Sanitize(user.Username))} is banned for: {Format.Code(ban.Reason, string.Empty)}");
        
        return Ok($"Nope. {Format.Bold(Format.Sanitize(user.Username))} is not banned.");
    }
}