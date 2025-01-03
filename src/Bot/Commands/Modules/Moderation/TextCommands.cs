namespace Volte.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("Say")]
    [Description("Bot repeats what you tell it to.")]
    public Task<ActionResult> SayAsync([Remainder, Description("What to say.")]
        string msg)
        => None(async () =>
        {
            await Context.CreateEmbed(msg).SendToAsync(Context.Channel);
            _ = await Context.Message.TryDeleteAsync();
        });

    [Command("SilentSay", "SSay")]
    [Description(
        "Runs the say command normally, but doesn't show the author in the message.")]
    public Task<ActionResult> SilentSayAsync([Remainder, Description("What to say.")]
        string msg)
        => None(async () =>
        {
            await new EmbedBuilder()
                .WithColor(Config.SuccessColor)
                .WithDescription(msg)
                .SendToAsync(Context.Channel);
            _ = await Context.Message.TryDeleteAsync();
        });

    [Command("PlainSay", "PSay")]
    [Description("Bot repeats what you tell it to; outside of an embed.")]
    public Task<ActionResult> SayPlainAsync([Remainder, Description("What to say.")]
        string msg)
        => None(() => Context.Channel.SendMessageAsync(msg, allowedMentions: AllowedMentions.None));
}