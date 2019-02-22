using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
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
            config.Blacklist.Add(phrase);
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Added **{phrase}** to the blacklist.").SendTo(Context.Channel);
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistRemoveAsync([Remainder] string phrase)
        {
            var config = Db.GetConfig(Context.Guild);
            if (config.Blacklist.ContainsIgnoreCase(phrase))
            {
                config.Blacklist.RemoveAt(config.Blacklist.FindIndex(p => p.EqualsIgnoreCase(phrase)));
                await Context.CreateEmbed($"Removed **{phrase}** from the word blacklist.").SendTo(Context.Channel);
                Db.UpdateConfig(config);
            }
            else
            {
                await Context.CreateEmbed($"**{phrase}** doesn't exist in the blacklist.").SendTo(Context.Channel);
            }
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
        [RequireGuildAdmin]
        public async Task BlacklistClearAsync()
        {
            var config = Db.GetConfig(Context.Guild);
            await Context.CreateEmbed($"Cleared the custom commands, containing **{config.Blacklist.Count}** words.")
                .SendTo(Context.Channel);
            config.Blacklist.Clear();
            Db.UpdateConfig(config);
        }
    }
}