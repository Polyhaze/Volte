using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    [Group("Blacklist", "Bl")]
    [RequireGuildAdmin]
    public class BlacklistModule : VolteModule
    {
        [Command, DummyCommand, Description("Command group for modifying this guild's phrase Blacklist.")]
        public async Task<ActionResult> BaseAsync() =>
            Ok(await CommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

        [Command("Add", "A")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        public Task<ActionResult> BlacklistAddAsync([Remainder, Description("The phrase to add to the blacklist.")]
            string phrase)
        {
            Context.Modify(data => data.Configuration.Moderation.Blacklist.Add(phrase));
            return Ok($"Added **{phrase}** to the blacklist.");
        }

        [Command("Remove", "Rem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        public Task<ActionResult> BlacklistRemoveAsync(
            [Remainder, Description("The phrase to remove from the blacklist.")]
            string phrase)
        {
            if (!Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
                return BadRequest($"**{phrase}** doesn't exist in the blacklist.");

            Context.Modify(data => data.Configuration.Moderation.Blacklist.Remove(phrase));
            return Ok($"Removed **{phrase}** from the word blacklist.");
        }

        [Command("Clear", "Cl")]
        [Description("Clears the blacklist for this guild.")]
        public Task<ActionResult> BlacklistClearAsync()
        {
            var count = Context.GuildData.Configuration.Moderation.Blacklist.Count;
            Context.Modify(data => data.Configuration.Moderation.Blacklist.Clear());
            return Ok($"Cleared the this guild's blacklist, containing {"word".ToQuantity(count)}.");
        }

        [Command("Action")]
        [Description(
            "Sets the action performed when a member uses a blacklisted word/phrase. I.e. says a swear, gets warned. Default is Nothing.")]
        [Remarks("Valid actions are `Nothing`, `Warn`, `Kick`, and `Ban`.")]
        public Task<ActionResult> BlacklistActionAsync(
            [Description("The action to be performed upon triggering the blacklist.")]
            BlacklistAction action)
        {
            Context.Modify(data => data.Configuration.Moderation.BlacklistAction = action);
            return action is BlacklistAction.Nothing
                ? Ok("Disabled punishing users for blacklist infractions.")
                : Ok($"Set {action} as the action performed when a member uses a blacklisted word/phrase.");
        }

        [Command("List", "Ls", "L")]
        [Description("Lists every single word/phrase inside of the blacklist.")]
        public Task<ActionResult> BlacklistListAsync()
            => Ok(Context.CreateEmbedBuilder()
                .WithTitle($"Blacklist for {Context.Guild.Name}")
                .WithDescription(Context.GuildData.Configuration.Moderation.Blacklist.IsEmpty()
                    ? "This guild has no words/phrases blacklisted."
                    : Context.GuildData.Configuration.Moderation.Blacklist.Select(x => Format.Code(x)).Join(", "))
            );
    }
}