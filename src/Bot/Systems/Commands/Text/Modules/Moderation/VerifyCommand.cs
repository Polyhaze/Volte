namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("Verify", "V")]
    [Description("Verifies a member.")]
    public async Task<ActionResult> VerifyAsync(
        [Remainder, EnsureNotSelf,
         Description("The user to verify; by removing the Unverified role and granting the Verified role.")]
        SocketGuildUser member)
    {
        var e = Context.CreateEmbedBuilder($"You've been verified in **{Context.Guild.Name}**.")
            .Apply(Context.GuildData);

        if (!await member.TrySendMessageAsync(embed: e.Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

        var uRole = Context.Guild.GetRole(Context.GuildData.Settings.Moderation.UnverifiedRole);
        var vRole = Context.Guild.GetRole(Context.GuildData.Settings.Moderation.VerifiedRole);
        if (uRole is null)
            return BadRequest(
                "This guild does not have an Unverified role set. Please use the VerifyRole command.");
        if (vRole is null)
            return BadRequest("This guild does not have a Verified role set. Please use the VerifyRole command.");
        if (member.HasRole(vRole.Id))
            return BadRequest("Member is already verified.");
        if (!member.HasRole(uRole.Id))
            return BadRequest($"Member does not have the Unverified role ({uRole.Mention}).");

        await member.RemoveRoleAsync(uRole);
        await member.AddRoleAsync(vRole);

        return Ok($"Successfully verified {member.Mention}.", 
            Context.ModAction(ModActionType.Verify).WithTarget(member)
        );
    }
}