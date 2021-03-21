using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        public Task<ActionResult> AvatarAsync([Remainder, Description("The user whose avatar you want to get. Defaults to yourself.")] SocketGuildUser user = null)
        {
            user ??= Context.User;
            return Ok(Context.CreateEmbedBuilder()
                .WithAuthor(user)
                .WithImageUrl(user.GetAvatarUrl(ImageFormat.Auto, 1024)));
        }
    }
}