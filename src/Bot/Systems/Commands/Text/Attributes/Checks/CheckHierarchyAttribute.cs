namespace Volte.Systems.Commands.Text;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
{
    public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
    {
        var ctx = context.Cast<VolteContext>();

        if (argument is RestUser ru && ctx.Guild.GetUser(ru.Id) is { } usr)
            argument = usr;

        if (argument is not SocketGuildUser u)
        {
            return Success(); //non-members should be considered hierarchically lower
        }

        return ctx.IsAdmin(u)
            ? CheckResult.Failed("Cannot ban someone with the configured Admin role.")
            : ctx.User.Hierarchy > u.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Failed("Cannot act on someone in a higher, or equal, hierarchy position than yourself.");
    }
}