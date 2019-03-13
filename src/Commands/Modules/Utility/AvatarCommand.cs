using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("Usage: $avatar [@user]")]
        public async Task AvatarAsync(SocketGuildUser user = null)
        {
            var u = user ?? Context.User;
            await Context.CreateEmbedBuilder()
                .WithImageUrl(u.GetAvatarUrl())
                .SendToAsync(Context.Channel);
        }
    }
}