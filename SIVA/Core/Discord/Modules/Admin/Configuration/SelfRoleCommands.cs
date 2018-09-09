using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration {
    class SelfRoleCommands : SIVACommand {
        public async Task SelfRoleAdd(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.SelfRoles.Add(roleName.ToLower());
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Successfully added **{roleName}** to the Self Roles for this server."));
        }

        public async Task SelfRoleRem(string roleName) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
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

        public async Task SelfRoleClear() {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
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