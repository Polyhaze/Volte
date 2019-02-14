using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Avatar")]
        [Description("Shows the mentioned user's avatar, or yours if no one is mentioned.")]
        [Remarks("Usage: $avatar [@user]")]
        public async Task Avatar(SocketGuildUser user = null) {
            var embed = Context.CreateEmbed(string.Empty).ToEmbedBuilder();
            if (user is null) {
                embed.WithImageUrl(Context.User.GetAvatarUrl());
            }
            else {
                embed.WithImageUrl(user.GetAvatarUrl());
            }

            await embed.SendTo(Context.Channel);

        }
    }
}