using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin.Configuration {
    class SelfRoleCommands : VolteCommand {
        [Command("SelfRoleAdd"), Alias("Sra")]
        public async Task SelfRoleAdd(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SelfRoles.Add(roleName.ToLower());
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Successfully added **{roleName}** to the Self Roles for this server."));
        }

        [Command("SelfRoleRem"), Alias("Srr")]
        public async Task SelfRoleRem(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            var selfRolesLower = new List<string>();

            foreach (var role in config.SelfRoles) {
                selfRolesLower.Add(role.ToLower());
            }

            if (selfRolesLower.Contains(roleName.ToLower())) {
                config.SelfRoles.Remove(roleName.ToLower());
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"Removed **{roleName}** from the Self Roles list on this server."));
            }
            else {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"The Self Roles list for this server doesn't contain **{roleName}**."));
            }
        }

        [Command("SelfRoleList"), Alias("Srl")]
        public async Task SelfRoleList() {
            var roleList = "";
            var config = ServerConfig.Get(Context.Guild);
            foreach (var role in config.SelfRoles) {
                var currentRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower().Equals(role.ToLower()));
                if (currentRole != null) {
                    roleList += $"**{currentRole.Name}**\n";
                }
            }

            await Context.Channel.SendMessageAsync("", false, CreateEmbed(Context, roleList));
        }

        [Command("SelfRoleClear"), Alias("Src")]
        public async Task SelfRoleClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SelfRoles.Clear();
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, "Successfully cleared all Self Roles for this server."));
        }
    }
}