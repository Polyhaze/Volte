using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("BlacklistAdd"), Alias("BlAdd")]
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
        public async Task BlacklistRemove([Remainder] string arg) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            if (config.Blacklist.Contains(arg)) {
                config.Blacklist.Remove(arg);
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
        public async Task BlacklistClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Cleared the custom commands, containing **{config.Blacklist.Count}** words."));
            config.Blacklist.Clear();
            Db.UpdateConfig(config);
        }
    }
}