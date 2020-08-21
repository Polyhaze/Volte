using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("ModProfile", "MP")]
        [Description("Shows a moderator relevant information about a user, or if no user is given, yourself.")]
        [Remarks("modprofile [Member]")]
        public Task<ActionResult> ModProfileAsync(DiscordMember user = null)
        {
            user ??= Context.Member;
            var ud = Context.GuildData.GetUserData(user.Id);

            var note = ud.Note.IsNullOrEmpty() ? "No note provided." : ud.Note;
                var e = Context.CreateEmbedBuilder()
                .WithAuthor($"{user}'s Moderator Profile", user.GetAvatarUrl(ImageFormat.Auto, 256))
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto, 512))
                .AddField("Username/Nickname", user.GetEffectiveUsername(), true)
                .AddField("Discriminator", user.Discriminator, true)
                .AddField("Can use Volte Mod Commands", user.IsModerator(Context), true)
                .AddField("Has ever been Kicked/Banned", ud.Actions.Any(x
                    => x.Type is ModActionType.Ban || x.Type is ModActionType.Kick || 
                       x.Type is ModActionType.Softban || x.Type is ModActionType.IdBan), true)
                .AddField("# of Warns", Context.GuildData.Extras.Warns.Count(x => x.User == user.Id), true)
                .AddField("Note", $"`{note}`", true)
                .WithFooter("Please note that the numbers shown above ONLY track bans/kicks done via Volte; if you've been banned manually it won't show up here.");

            return Ok(e);

        }        
    }
}