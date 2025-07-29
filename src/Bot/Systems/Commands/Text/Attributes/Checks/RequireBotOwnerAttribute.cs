namespace Volte.Systems.Commands.Text;

public sealed class RequireBotOwnerAttribute : CheckAttribute
{
    public override ValueTask<CheckResult> CheckAsync(CommandContext context)
    {
        var ctx = context.Cast<VolteContext>();
        if (ctx.User.IsBotOwner()) return CheckResult.Successful;
            
        return CheckResult.Failed("Insufficient permission.");
    }
}