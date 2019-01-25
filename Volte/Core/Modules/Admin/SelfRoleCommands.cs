using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("SelfRoleAdd"), Alias("Sra")]
        public async Task SelfRoleAdd(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.Add(roleName.ToLower());
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Successfully added **{roleName}** to the Self Roles for this server."));
        }

        [Command("SelfRoleRem"), Alias("Srr")]
        public async Task SelfRoleRem(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            var selfRolesLower = new List<string>();

            foreach (var role in config.SelfRoles) {
                selfRolesLower.Add(role.ToLower());
            }

            if (selfRolesLower.Contains(roleName.ToLower())) {
                config.SelfRoles.Remove(roleName.ToLower());
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"Removed **{roleName}** from the Self Roles list on this server."));
                Db.UpdateConfig(config);
            }
            else {
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"The Self Roles list for this server doesn't contain **{roleName}**."));
            }
        }

        [Command("SelfRoleList"), Alias("Srl")]
        public async Task SelfRoleList() {
            var roleList = "";
            var config = Db.GetConfig(Context.Guild);
            config.SelfRoles.ForEach(role => {
                var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower().Equals(role.ToLower()));
                roleList += $"**{currentRole?.Name}**\n";
            });

            await Reply(Context.Channel, CreateEmbed(Context, roleList));
        }

        [Command("SelfRoleClear"), Alias("Src")]
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