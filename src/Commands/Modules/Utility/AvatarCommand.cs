using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

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

            string FormatEmbedString(params ushort[] sizes) => new StringBuilder().Apply(sb =>
            {
                sb.Append(sizes.Take(1)
                    .Select(x => $"{Format.Url(x.ToString(), user.GetEffectiveAvatarUrl(size: x))} ").First());
                sb.Append(sizes.Skip(1)
                    .Select(x => $"| {Format.Url(x.ToString(), user.GetEffectiveAvatarUrl(size: x))} ").Join(string.Empty));
            }).ToString().Trim();


            return Ok(Context.CreateEmbedBuilder(FormatEmbedString(128, 256, 512, 1024))
                .WithAuthor(user)
                .WithImageUrl(user.GetEffectiveAvatarUrl()));
        }
    }
}