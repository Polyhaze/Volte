using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("blacklistadd {String}")]
        public Task<ActionResult> BlacklistAddAsync([Remainder] string phrase)
        {
            if (!Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                ModifyData(data =>
                {
                    data.Configuration.Moderation.Blacklist.Add(phrase);
                    return data;
                });
                return Ok($"Added **{phrase}** to the blacklist.");
            }

            return BadRequest($"**{phrase} is already in the blacklist.**");
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("blacklistremove {String}")]
        public Task<ActionResult> BlacklistRemoveAsync([Remainder] string phrase)
        {
            if (Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
            {
                var i = Context.GuildData.Configuration.Moderation.Blacklist.IndexOf(phrase);
                ModifyData(data =>
                {
                    data.Configuration.Moderation.Blacklist.RemoveAt(i);
                    return data;
                });
            }

            return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("blacklistclear")]
        public Task<ActionResult> BlacklistClearAsync()
        {
            var count = Context.GuildData.Configuration.Moderation.Blacklist.Count;
            ModifyData(data =>
            {
                data.Configuration.Moderation.Blacklist.Clear();
                return data;
            });
            return Ok(
                $"Cleared the this guild's blacklist, containing **{count}** words.");
        }

        [Command("BlacklistAction", "BlA")]
        [Description("Sets the action performed when a member uses a blacklisted word/phrase. I.e. says a swear, gets warned. Default is Nothing.")]
        [Remarks("blacklistaction {nothing/warn/kick/ban}")]
        public Task<ActionResult> BlacklistActionAsync(string input)
        {
            var action = BlacklistActions.DetermineAction(input);

            ModifyData(data =>
            {
                data.Configuration.Moderation.BlacklistAction = action;
                return data;
            });


            return Ok($"Set **{action}** as the action performed when a member uses a blacklisted word/phrase.");
        }
    }
}