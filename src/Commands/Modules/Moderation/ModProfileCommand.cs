using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core;
using Volte.Core.Models;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule
    {
        public async Task ModProfileAsync(SocketGuildUser user = null)
        {
            user ??= Context.User;
            var ud = Context.GuildData.GetUserData(user.Id);

            var e = Context.CreateEmbedBuilder()
                .WithColor(user.Roles.OrderByDescending(x => x.Position)
                    .FirstOrDefault()?.Color ?? new Color(Config.SuccessColor))
                .WithThumbnailUrl(user.GetAvatarUrl(size: 512))
                .WithTitle($"Moderator Profile for {user}")
                .AddField("Username/Nickname", user.GetEffectiveUsername(), true)
                .AddField("Discriminator", user.Discriminator, true)
                .AddField("Can use Volte Mod Commands", user.IsModerator(Context))
                .AddField("Has been Kicked/Banned", ud.Actions.Any(x
                    => x.Type is ModActionType.Ban || x.Type is ModActionType.Kick || x.Type is ModActionType.Softban ||
                       x.Type is ModActionType.IdBan));

        }        
    }
}