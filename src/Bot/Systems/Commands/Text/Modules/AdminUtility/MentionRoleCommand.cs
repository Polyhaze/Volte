namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class AdminUtilityModule
{
    [Command("MentionRole", "Menro")]
    [Description(
        "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
    public Task<ActionResult> MentionRoleAsync([Remainder, Description("The role to mention.")]
        SocketRole role)
    {
        return role.IsMentionable
            ? Ok(role.Mention, shouldEmbed: false)
            : Ok(async () =>
            {
                await role.ModifyAsync(static x => x.Mentionable = true);
                await Context.Channel.SendMessageAsync(role.Mention);
                await role.ModifyAsync(static x => x.Mentionable = false);
            });
    }
}