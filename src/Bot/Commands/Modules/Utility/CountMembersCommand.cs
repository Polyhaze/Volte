namespace Volte.Commands.Text.Modules;

public partial class UtilityModule
{
    [Command("CountMembers", "Cm")]
    [Description("Counts the amount of members in the given role.")]
    public async Task<ActionResult> CountMembersAsync(
        [Remainder, Description("The role in which you want to count members for.")]
        SocketRole role = null)
    {
        var users = (await Context.Guild.GetUsersAsync().FlattenAsync())
            .Where(x => x.RoleIds.Contains(role?.Id ?? Context.Guild.Id)) // the guild ID and everyone role are the same
            .ToArray();

        return Ok(sb =>
        {
            sb.Append($"There {"is".ToQuantity(users.Length).Split(" ")[1]} {"member".ToQuantity(users.Length)} ");
            sb.Append(role is null ? "in the guild" : $"in the role {role.Mention}");
            sb.Append(users.Any(x => x.Id == Context.User.Id) && role is not null
                ? "; including you."
                : ".");
        });
    }
}