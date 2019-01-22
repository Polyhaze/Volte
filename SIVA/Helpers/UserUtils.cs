using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using System.Linq;
using Discord;
using Discord.Commands;
using SIVA.Core.Discord;
using SIVA.Core.Modules;

namespace SIVA.Helpers {
    public static class UserUtils {
        /// <summary>
        ///     Checks if the user given is the bot owner.
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <returns>System.Boolean</returns>
        public static bool IsBotOwner(IUser user) {
            return user.Id == Config.GetOwner();
        }

        /// <summary>
        ///     Checks if a SocketUser is the owner of the given SocketGuild.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns>System.Boolean</returns>
        public static bool IsServerOwner(IUser user, IGuild guild) {
            return guild.OwnerId.Equals(user.Id);
        }

        /// <summary>
        ///     Checks if the given SocketGuildUser has the given SocketRole.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>System.Boolean</returns>
        public static bool HasRole(IGuildUser user, IRole role) {
            return user.RoleIds.Contains(role.Id);
        }

        /// <summary>
        ///     Checks if the given SocketGuildUser has the given SocketRole Id.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleId"></param>
        /// <returns>System.Boolean</returns>
        public static bool HasRole(IGuildUser user, ulong roleId) {
            return user.RoleIds.Contains(roleId);
        }

        /// <summary>
        ///     Checks if the user is an admin in the given context.
        ///     If a server owner has not set their admin role, this will always return true.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>System.Boolean</returns>
        public static bool IsAdmin(SIVAContext ctx) {
            var config = ServerConfig.Get(ctx.Guild);
            var adminRole = ctx.Guild.Roles.FirstOrDefault(r => r.Id == config.AdminRole);
            return adminRole != null && ((SocketGuildUser) ctx.User).Roles.Contains(adminRole);
        }
    }
}