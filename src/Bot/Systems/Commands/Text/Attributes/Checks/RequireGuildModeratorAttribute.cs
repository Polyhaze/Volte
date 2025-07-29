namespace Volte.Systems.Commands.Text;

public sealed class RequireGuildModeratorAttribute : CheckAttribute
{
    public override ValueTask<CheckResult> CheckAsync(CommandContext context)
    {
        var ctx = context.Cast<VolteContext>();
        if (ctx.IsModerator(ctx.User)) return CheckResult.Successful;
            
        return CheckResult.Failed("Insufficient permission.");
    }
}