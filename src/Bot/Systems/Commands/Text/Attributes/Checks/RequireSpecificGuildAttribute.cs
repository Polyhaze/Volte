namespace Volte.Systems.Commands.Text;

public class RequireSpecificGuildAttribute(ulong guildId) : CheckAttribute
{
    public ulong GuildId => guildId;
    
    public override ValueTask<CheckResult> CheckAsync(CommandContext context)
    {
        var ctx = context.Cast<VolteContext>();
        if (ctx.Guild.Id == guildId) 
            return CheckResult.Successful;
            
        return CheckResult.Failed("Insufficient permission.");
    }
}