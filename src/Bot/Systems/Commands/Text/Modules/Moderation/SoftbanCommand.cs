namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class ModerationModule
{
    [Command("Softban")]
    [Description(
        "Softbans the mentioned user, kicking them and deleting the last x (where x is defined by the daysToDelete parameter) days of messages.")]
    public async Task<ActionResult> SoftBanAsync(
        [CheckHierarchy, EnsureNotSelf, Description("The member to softban.")]
        SocketGuildUser user, [Description("The amount of days of messages to delete.")]
        int daysToDelete = 7,
        [Remainder, Description("The reason for the softban.")]
        string reason = "Softbanned by a Moderator.")
    {
        
        var e = Context.CreateEmbedBuilder(reason is not null
            ? $"You've been banned from **{Context.Guild.Name}** for **{reason}**."
            : $"You've been banned from **{Context.Guild.Name}**."
        );
        
        if (!await user.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {user}!");

        var originalReason = reason;

        reason = reason is null 
            ? GetDefaultReason("Banned", Context.User) 
            : GetReason(Context.User, reason, out originalReason);

        try
        {
            await user.BanAsync(daysToDelete, reason);
            await Context.Guild.RemoveBanAsync(user);

            return Ok($"Successfully softbanned **{user}**.", _ =>
                ModerationService.OnModActionCompleteAsync(ModActionEventArgs
                    .InContext(Context)
                    .WithActionType(ModActionType.Softban)
                    .WithTarget(user)
                    .WithReason(originalReason))
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred softbanning that user. Do I have permission; or are they higher than me in the role list?");
        }
    }
}