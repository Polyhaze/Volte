using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Group("Blacklist", "Bl")]
        [RequireGuildAdmin]
        public sealed class BlacklistModule : VolteModule
        {
            [Command("Add")]
            [Description("Adds a given word/phrase to the blacklist for this guild.")]
            [Remarks("blacklist add {String}")]
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

            [Command("Remove", "Rem")]
            [Description("Removes a given word/phrase from the blacklist for this guild.")]
            [Remarks("blacklist remove {String}")]
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

            [Command("Clear", "Cl")]
            [Description("Clears the blacklist for this guild.")]
            [Remarks("blacklist clear")]
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

            [Command("Action", "A")]
            [Description(
                "Sets the action performed when a member uses a blacklisted word/phrase. I.e. says a swear, gets warned. Default is Nothing.")]
            [Remarks("blacklist action {nothing/warn/kick/ban}")]
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
}