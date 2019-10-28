using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistAddAsync([Remainder] string phrase)
        {
            Context.GuildData.Configuration.Moderation.Blacklist.Add(phrase);
            Db.UpdateData(Context.GuildData);
            return Ok($"Added **{phrase}** to the blacklist.");
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistRemoveAsync([Remainder] string phrase)
        {
            if (Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                Context.GuildData.Configuration.Moderation.Blacklist.Remove(phrase);
                Db.UpdateData(Context.GuildData);
                return Ok($"Removed **{phrase}** from the word blacklist.");
            }

            return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("blacklistclear")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistClearAsync()
        {
            var count = Context.GuildData.Configuration.Moderation.Blacklist.Count;
            Context.GuildData.Configuration.Moderation.Blacklist.Clear();
            Db.UpdateData(Context.GuildData);
            return Ok(
                $"Cleared the this guild's blacklist, containing **{count}** words.");
        }
    }
}