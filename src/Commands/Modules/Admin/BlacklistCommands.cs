using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Gommon;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistAddAsync([Remainder] string phrase)
        {
            var config = Db.GetConfig(Context.Guild);
            config.ModerationOptions.Blacklist.Add(phrase);
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Added **{phrase}** to the blacklist.").SendToAsync(Context.Channel);
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistRemoveAsync([Remainder] string phrase)
        {
            var config = Db.GetConfig(Context.Guild);
            if (config.ModerationOptions.Blacklist.ContainsIgnoreCase(phrase))
            {
                config.ModerationOptions.Blacklist.Remove(phrase);
                await Context.CreateEmbed($"Removed **{phrase}** from the word blacklist.")
                    .SendToAsync(Context.Channel);
                Db.UpdateConfig(config);
            }
            else
            {
                await Context.CreateEmbed($"**{phrase}** doesn't exist in the blacklist.").SendToAsync(Context.Channel);
            }
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
        [RequireGuildAdmin]
        public async Task BlacklistClearAsync()
        {
            var config = Db.GetConfig(Context.Guild);
            await Context
                .CreateEmbed(
                    $"Cleared the custom commands, containing **{config.ModerationOptions.Blacklist.Count}** words.")
                .SendToAsync(Context.Channel);
            config.ModerationOptions.Blacklist.Clear();
            Db.UpdateConfig(config);
        }
    }
}