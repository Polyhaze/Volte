namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("PirateBan", "PBan")]
    [Description("Bans the user with a very long-winded message as to why piracy is not supported. Content is retrieved from the 'emulationisnotpiracy' tag.")]
    [RequireSpecificGuild(1294443224030511104)]
    public async Task<ActionResult> PirateBanAsync(
        [CheckHierarchy, EnsureNotSelf, Description("The target user.")] SocketGuildUser member)
    {
        var e = Context.CreateEmbedBuilder($"You've been banned from **{Context.Guild.Name}** for **piracy**.");
        
        if (!await member.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

        var tag = Context.GuildData.GetTagByNameOrAlias("emulationisnotpiracy");
        if (tag.HasValue)
        {
            if (!await member.TrySendMessageAsync(text: tag.Value.Response))
                Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
        }

        try
        {
            await member.BanAsync(7, GetReason("Banned", Context.User, "Piracy"));
            return Ok($"Successfully banned **{member}** from this guild.", Context.ModAction(ModActionType.Ban)
                .WithTarget(member)
                .WithReason("Piracy")
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}