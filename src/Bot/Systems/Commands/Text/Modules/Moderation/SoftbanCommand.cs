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

        try
        {
            await user.BanAsync(daysToDelete, GetReason("Softbanned", Context.User, reason));
            await Context.Guild.RemoveBanAsync(user, new RequestOptions { AuditLogReason = "Second half of softban action" });

            return Ok($"Successfully softbanned **{user}**.", Context.ModAction
                .WithActionType(ModActionType.Softban)
                .WithTarget(user)
                .WithReason(reason)
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred softbanning that user. Do I have permission; or are they higher than me in the role list?");
        }
    }
}