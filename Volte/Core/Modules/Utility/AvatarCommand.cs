using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Avatar")]
        public async Task Avatar(SocketGuildUser user = null) {
            var embed = CreateEmbed(Context, string.Empty).ToEmbedBuilder();
            if (user == null) {
                embed.WithImageUrl(Context.User.GetAvatarUrl());
            }
            else {
                embed.WithImageUrl(user.GetAvatarUrl());
            }
            
            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }
    }
}