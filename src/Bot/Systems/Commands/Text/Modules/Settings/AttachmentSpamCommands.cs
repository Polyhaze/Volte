using ActionType = Volte.Systems.UserFilter.ActionType;

namespace Volte.Systems.Commands.Text.Modules;

public partial class SettingsModule
{
    [Command("AttachmentSpam")]
    [Description(
        "Enables or disables the attachment spam (aka \"Mr. Beast\" scam) bot detection. This command is interactive if you are enabling the system.")]
    public Task<ActionResult> AttachmentSpam(bool enabled)
    {
        // ReSharper disable once InvertIf
        if (!enabled)
        {
            Context.Modify(data => data.Settings.Moderation.AttachmentSpamDetection = false);
            return Ok("Attachment spam detection has been disabled.");
        }

        return Ok(async () =>
        {
            GetActionType:
            await Context
                .CreateEmbed("What would you like to do to a user when attachment spam is detected? (Kick, SoftBan, Ban)")
                .SendToAsync(Context.Channel);
            var (actionType, didTimeout, _) = await Context.GetNextEnumAsync<ActionType>();
            if (didTimeout) return;
            if (!actionType.HasValue || actionType.Check(a => a == ActionType.Warn)) goto GetActionType;

            GetReason:
            await Context.CreateEmbed("What would you like the reason to be for the moderation action?")
                .SendToAsync(Context.Channel);
            (var message, didTimeout) = await Context.GetNextAsync();
            if (didTimeout) return;
            if (!message.HasValue) goto GetReason;

            Context.Modify(data =>
            {
                data.Settings.Moderation.AttachmentSpamDetection = true;
                data.Settings.Moderation.AttachmentSpamAction = actionType.Value;
                data.Settings.Moderation.AttachmentSpamReason = message.Value.Content;
            });

            await Context.CreateEmbed("Attachment spam detection has been configured successfully.").SendToAsync(Context.Channel);
        }, false);
    }
}