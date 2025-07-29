namespace Volte.Systems.Commands.Text;

[InjectTypeParser]
public sealed class RoleParser : VolteTypeParser<SocketRole>
{
    public override ValueTask<TypeParserResult<SocketRole>> ParseAsync(string value, VolteContext ctx) =>
        Parse(value, ctx);
    
    public static TypeParserResult<SocketRole> Parse(string value, VolteContext ctx)
    {
        SocketRole role = default;
        if (value.TryParse<ulong>(out var id) || MentionUtils.TryParseRole(value, out id))
            role = ctx.Guild.GetRole(id).Cast<SocketRole>();

        if (role is null)
        {
            var match = ctx.Guild.Roles.Where(x => x.Name.EqualsIgnoreCase(value)).ToList();
            if (match.Count > 1)
                return Failure(
                    "Multiple roles found. Try mentioning the role or using its ID.");

            role = match.FirstOrDefault().Cast<SocketRole>();
        }

        return role is null
            ? Failure($"Role {Format.Code(value)} not found.")
            : Success(role);
    }
}