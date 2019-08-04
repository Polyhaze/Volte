using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistAddAsync([Remainder] string phrase)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.Blacklist.Add(phrase);
            Db.UpdateData(data);
            return Ok($"Added **{phrase}** to the blacklist.");
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistRemoveAsync([Remainder] string phrase)
        {
            var data = Db.GetData(Context.Guild);
            if (data.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                data.Configuration.Moderation.Blacklist.Remove(phrase);
                Db.UpdateData(data);
                return Ok($"Removed **{phrase}** from the word blacklist.");
            }

            return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
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