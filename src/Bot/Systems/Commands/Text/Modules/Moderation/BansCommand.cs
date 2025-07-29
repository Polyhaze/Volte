using Volte.Systems.Interactive;

namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Bans")]
    [Description("Shows all bans in this guild.")]
    public async Task<ActionResult> BansAsync()
    {
        var banList = (await Context.Guild.GetBansAsync().FlattenAsync()).ToList();

        if (banList.Count > 0)
            return Ok(
                new PaginatedMessage.Builder()
                    .WithTitle($"Bans in {Context.Guild.Name}")
                    .WithPages(banList.Select(static b => $"**{b.User}**: {Format.Code(b.Reason ?? "No reason provided.")}"))
                    .SplitPages(25)
            );
        
        return BadRequest("This guild doesn't have anyone banned.");
    }
}