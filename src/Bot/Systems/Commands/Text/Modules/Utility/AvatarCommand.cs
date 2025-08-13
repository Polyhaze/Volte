namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Avatar")]
    [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
    public Task<ActionResult> AvatarAsync(
        [Remainder, Description("The user whose avatar you want to get. Defaults to yourself.")]
        SocketGuildUser user = null)
    {
        user ??= Context.User;

        return Ok(Context.CreateEmbedBuilder(formatAvatarSizesToUrls(128, 256, 512, 1024))
            .WithAuthor(user)
            .WithImageUrl(user.GetEffectiveAvatarUrl()));
        
        string formatAvatarSizesToUrls(params ushort[] sizes) => 
            sizes.Select(x => $" {Format.Url(x.ToString(), user.GetEffectiveAvatarUrl(size: x))} ")
                .JoinToString('|').Trim();
    }
}