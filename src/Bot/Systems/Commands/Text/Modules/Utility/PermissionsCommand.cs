namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Permissions", "Perms")]
    [Description("Shows someone's, or the command invoker's, permissions in the current guild.")]
    public Task<ActionResult> PermissionsAsync(
        [Remainder, Description("The user to show permissions for. Defaults to yourself.")]
        SocketGuildUser user = null)
    {
        user ??= Context.User; // get the user (or the invoker, if none specified)


        if (user.Id == Context.Guild.OwnerId)
            return Ok("User is owner of this guild, and has all permissions.");

        if (user.GuildPermissions.Administrator)
            return Ok("User has Administrator permission, and has all permissions.");


        var (allowed, disallowed) = GetPermissions(user);

        var allowedString = allowed.Select(static a => $"- {a.Name}").JoinToString('\n');
        var disallowedString = disallowed.Select(static a => $"- {a.Name}").JoinToString('\n');
        return Ok(Context.CreateEmbedBuilder().WithAuthor(user)
            .AddField("Allowed", allowedString.IsNullOrEmpty() ? "- None" : allowedString, true)
            .AddField("Denied", disallowedString.IsNullOrEmpty() ? "- None" : disallowedString, true));
    }
}