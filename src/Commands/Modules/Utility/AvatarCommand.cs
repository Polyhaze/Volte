using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        public Task<ActionResult> AvatarAsync(
            [Remainder, Description("The user whose avatar you want to get. Defaults to yourself.")]
            SocketGuildUser user = null)
        {
            user ??= Context.User;
            if (user.GetAvatarUrl() is null) return BadRequest("User has a default avatar.");
            return Ok(Context.CreateEmbedBuilder()
                .WithDescription($"{Format.Url("128", user.GetAvatarUrl(size: 128))} " +
                                 $"| {Format.Url("256", user.GetAvatarUrl(size: 256))} " +
                                 $"| {Format.Url("512", user.GetAvatarUrl(size: 512))} " +
                                 $"| {Format.Url("1024", user.GetAvatarUrl(size: 1024))}")
                .WithAuthor(user)
                .WithImageUrl(user.GetAvatarUrl()));
        }
    }
}