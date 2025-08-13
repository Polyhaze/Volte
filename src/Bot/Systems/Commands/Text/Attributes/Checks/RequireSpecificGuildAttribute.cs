namespace Volte.Systems.Commands.Text;

public class RequireSpecificGuildAttribute(ulong[] guildIds) : CheckAttribute
{
    public ulong[] GuildIds => guildIds;
    
    public override ValueTask<CheckResult> CheckAsync(CommandContext context)
    {
        var ctx = context.Cast<VolteContext>();
        if (GuildIds.Contains(ctx.Guild.Id))
            return CheckResult.Successful;
            
        return CheckResult.Failed("Insufficient permission.");
    }
}