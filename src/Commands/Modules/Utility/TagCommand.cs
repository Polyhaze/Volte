using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Tag")]
        [Description("Gets a tag's contents if it exists.")]
        public Task<ActionResult> TagAsync([Remainder, Description("The tag to show.")] Tag tag)
        {
            tag.Uses += 1;
            Db.Save(Context.GuildData);

            if (Context.GuildData.Configuration.EmbedTagsAndShowAuthor)
                return Ok(Context.CreateEmbedBuilder(tag.FormatContent(Context)).WithAuthor(author: null).WithFooter($"Requested by {Context.User}."));

            return Ok(tag.FormatContent(Context), shouldEmbed: false);

        }
    }
}