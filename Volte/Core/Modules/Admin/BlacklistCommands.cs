using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("BlacklistAdd"), Alias("BlAdd")]
        [Summary("Adds a given word/phrase to the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistadd {phrase}")]
        public async Task BlacklistAdd([Remainder] string arg) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);

            config.Blacklist.Add(arg);
            Db.UpdateConfig(config);
            await Reply(Context.Channel, CreateEmbed(Context, $"Added **{arg}** to the blacklist."));
        }

        [Command("BlacklistRemove"), Alias("BlRem")]
        [Summary("Removes a given word/phrase from the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistremove {phrase}")]
        public async Task BlacklistRemove([Remainder] string arg) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            if (config.Blacklist.Any(p => p.EqualsIgnoreCase(arg))) {
                config.Blacklist.RemoveAt(config.Blacklist.FindIndex(p => p.EqualsIgnoreCase(arg)));
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"Removed **{arg}** from the word blacklist."));
                Db.UpdateConfig(config);
            }
            else {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"**{arg}** doesn't exist in the blacklist."));
            }
        }

        [Command("BlacklistClear"), Alias("BlCl")]
        [Summary("Clears the blacklist for this guild.")]
        [Remarks("Usage: |prefix|blacklistclear")]
        public async Task BlacklistClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Cleared the custom commands, containing **{config.Blacklist.Count}** words."));
            config.Blacklist.Clear();
            Db.UpdateConfig(config);
        }
    }
}