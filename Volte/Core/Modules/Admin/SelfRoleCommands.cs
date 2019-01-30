using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("SelfRoleAdd"), Alias("Sra")]
        [Summary("Adds a role to the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfroleadd {roleName}")]
        public async Task SelfRoleAdd(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.Add(roleName);
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Successfully added **{roleName}** to the Self Roles for this guild."));
        }

        [Command("SelfRoleRem"), Alias("Srr")]
        [Summary("Removes a role from the list of self roles for this guild.")]
        [Remarks("Usage: |prefix|selfrolerem {roleName}")]
        public async Task SelfRoleRem(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);

            if (config.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName))) {
                config.SelfRoles.Remove(roleName);
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"Removed **{roleName}** from the Self Roles list on this guild."));
                Db.UpdateConfig(config);
            }
            else {
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"The Self Roles list for this guild doesn't contain **{roleName}**."));
            }
        }

        [Command("SelfRoleClear"), Alias("Src")]
        [Summary("Clears the self role list for this guild.")]
        [Remarks("Usage: |prefix|selfroleclear")]
        public async Task SelfRoleClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.Clear();
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context, "Successfully cleared all Self Roles for this server."));
        }
    }
}