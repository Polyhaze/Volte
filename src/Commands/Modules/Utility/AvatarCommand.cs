using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("avatar [User]")]
        public Task<ActionResult> AvatarAsync(SocketGuildUser user = null)
        {
            user ??= Context.User;
            return Ok(Context.CreateEmbedBuilder()
                .WithAuthor(user)
                .WithImageUrl(user.GetAvatarUrl(ImageFormat.Auto, 1024)));
        }
    }
}