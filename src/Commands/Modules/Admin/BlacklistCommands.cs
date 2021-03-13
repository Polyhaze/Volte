using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("blacklistadd {String}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistAddAsync([Remainder] string phrase)
        {
            Context.GuildData.Configuration.Moderation.Blacklist.Add(phrase);
            Db.UpdateData(Context.GuildData);
            return Ok($"Added **{phrase}** to the blacklist.");
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("blacklistremove {String}")]
        [RequireGuildAdmin]
        public Task<ActionResult> BlacklistRemoveAsync([Remainder] string phrase)
        {
            if (!Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
                return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
            
            Context.GuildData.Configuration.Moderation.Blacklist.Remove(phrase);
            Db.UpdateData(Context.GuildData);
            return Ok($"Removed **{phrase}** from the word blacklist.");

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

        [Command("BlacklistAction", "BlA")]
        [Description("Sets the action performed when a member uses a blacklisted word/phrase. I.e. says a swear, gets warned. Default is Nothing.")]
        [Remarks("blacklistaction {nothing/warn/kick/ban}")]
        public Task<ActionResult> BlacklistActionAsync(string input)
        {
            Context.GuildData.Configuration.Moderation.BlacklistAction = BlacklistActions.DetermineAction(input);
            Db.UpdateData(Context.GuildData);
            return Ok($"Set {input} as the action performed when a member uses a blacklisted word/phrase.");
        }

        [Command("BlacklistList", "BlL")]
        [Description("Lists every single word/phrase inside of the blacklist.")]
        [Remarks("blacklistlist")]
        public Task<ActionResult> BlacklistListAsync()
        {
            return Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Blacklist for {Context.Guild.Name}")
                .WithDescription(Context.GuildData.Configuration.Moderation.Blacklist.IsEmpty() 
                    ? "This guild has no words/phrases blacklisted." 
                    : $"`{Context.GuildData.Configuration.Moderation.Blacklist.Join("`, `")}`")
            );
        }
    }
}