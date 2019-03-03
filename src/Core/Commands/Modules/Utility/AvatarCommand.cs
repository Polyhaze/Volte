using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("Usage: $avatar [@user]")]
        public async Task AvatarAsync(SocketGuildUser user = null)
        {
            var embed = Context.CreateEmbedBuilder()
                .WithImageUrl(user is null ? Context.User.GetAvatarUrl() : user.GetAvatarUrl());

            await embed.SendTo(Context.Channel);
        }
    }
}