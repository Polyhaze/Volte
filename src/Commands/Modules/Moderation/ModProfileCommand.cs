using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Attributes;
using Volte.Core.Models;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule
    {
        [Command("ModProfile", "MP")]
        [Description("Shows a moderator relevant information about a user, or if no user is given, yourself.")]
        [Remarks("modprofile [User]")]
        [RequireGuildModerator]
        public Task<ActionResult> ModProfileAsync(SocketGuildUser user = null)
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
                .AddField("Can use Volte Mod Commands", user.IsModerator(Context), true)
                .AddField("Has been Kicked/Banned", ud.Actions.Any(x
                    => x.Type is ModActionType.Ban || x.Type is ModActionType.Kick || x.Type is ModActionType.Softban ||
                       x.Type is ModActionType.IdBan), true)
                .AddField("# of Warns", Context.GuildData.Extras.Warns.Count(x => x.User == user.Id), true)
                .AddField("Note", $"`{ud.Note}`", true);

            return Ok(e);

        }        
    }
}