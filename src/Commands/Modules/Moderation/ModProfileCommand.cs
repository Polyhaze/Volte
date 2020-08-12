using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
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

            var note = ud.Note.IsNullOrEmpty() ? "No note provided." : ud.Note;
                var e = Context.CreateEmbedBuilder()
                .WithAuthor($"{user}'s Moderator Profile", user.GetAvatarUrl())
                .WithThumbnailUrl(user.GetAvatarUrl(size: 512))
                .AddField("Username/Nickname", user.GetEffectiveUsername(), true)
                .AddField("Discriminator", user.Discriminator, true)
                .AddField("Can use Volte Mod Commands", user.IsModerator(Context), true)
                .AddField("Has been Kicked/Banned", ud.Actions.Any(x
                    => x.Type is ModActionType.Ban || x.Type is ModActionType.Kick || 
                       x.Type is ModActionType.Softban || x.Type is ModActionType.IdBan), true)
                .AddField("# of Warns", Context.GuildData.Extras.Warns.Count(x => x.User == user.Id), true)
                .AddField("Note", $"`{note}`", true);

            return Ok(e);

        }        
    }
}