using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Note")]
        [Description("Adds a note to a user to let other moderators know something relevant about them.")]
        [Remarks("note {Member} [String]")]
        public Task<ActionResult> NoteAsync(DiscordMember user, [Remainder] string note = null)
        {
            var userNote = Context.GuildData.GetUserData(user.Id).Note;
            if (note is null)
            {
                return Ok(userNote.IsNullOrEmpty() ? "No note provided." : userNote);
            }
            
            Context.GuildData.GetUserData(user.Id).Note = note;
            Db.UpdateData(Context.GuildData);

                return userNote.IsNullOrEmpty()
                ? Ok($"Successfully set **{user}**'s note to `{note}`")
                : Ok($"Changed **{user}**'s note from `{userNote}` to `{note}`");
        }
    }
}