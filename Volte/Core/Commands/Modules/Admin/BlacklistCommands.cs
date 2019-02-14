using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("BlacklistAdd", "BlAdd")]
        [Description("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistadd {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistAdd([Remainder] string arg) {
            var config = Db.GetConfig(Context.Guild);
            config.Blacklist.Add(arg);
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Added **{arg}** to the blacklist.").SendTo(Context.Channel);
        }

        [Command("BlacklistRemove", "BlRem")]
        [Description("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        [RequireGuildAdmin]
        public async Task BlacklistRemove([Remainder] string arg) {
            var config = Db.GetConfig(Context.Guild);
            if (config.Blacklist.Any(p => p.EqualsIgnoreCase(arg))) {
                config.Blacklist.RemoveAt(config.Blacklist.FindIndex(p => p.EqualsIgnoreCase(arg)));
                await Context.CreateEmbed($"Removed **{arg}** from the word blacklist.").SendTo(Context.Channel);
                Db.UpdateConfig(config);
            }
            else {
                await Context.CreateEmbed($"**{arg}** doesn't exist in the blacklist.").SendTo(Context.Channel);
            }
        }

        [Command("BlacklistClear", "BlCl")]
        [Description("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
        [RequireGuildAdmin]
        public async Task BlacklistClear() {
            var config = Db.GetConfig(Context.Guild);
            await Context.CreateEmbed($"Cleared the custom commands, containing **{config.Blacklist.Count}** words.")
                .SendTo(Context.Channel);
            config.Blacklist.Clear();
            Db.UpdateConfig(config);
        }
    }
}