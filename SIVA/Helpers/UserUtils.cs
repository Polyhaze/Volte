using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using System.Linq;
using Discord.Commands;

namespace SIVA.Helpers
{
    public class UserUtils
    {
        /// <summary>
        ///     Checks if the user given is the bot owner.
        /// </summary>
        /// <param name="user">User to check.</param>
        /// <returns>System.Boolean</returns>
        
        public static bool IsBotOwner(SocketUser user)
        {
            return user.Id == Config.GetOwner();
        }
        
        /// <summary>
        ///     Checks if a SocketUser is the owner of the given SocketGuild.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns>System.Boolean</returns>

        public static bool IsServerOwner(SocketUser user, SocketGuild guild)
        {
            return guild.OwnerId.Equals(user.Id);
        }

        /// <summary>
        ///     Checks if the given SocketGuildUser has the given SocketRole.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>System.Boolean</returns>
        
        public static bool HasRole(SocketGuildUser user, SocketRole role)
        {
            return user.Roles.Contains(role);
        }

        /// <summary>
        ///     Checks if the given SocketGuildUser has the given SocketRole Id.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleId"></param>
        /// <returns>System.Boolean</returns>
        
        public static bool HasRole(SocketGuildUser user, ulong roleId)
        {
            return user.Roles.Contains(user.Guild.Roles.First(r => r.Id == roleId));
        }
        
        /// <summary>
        ///     Checks if the user is an admin in the given context.
        ///     If a server owner has not set their admin role, this will always return true.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>System.Boolean</returns>

        public static bool IsAdmin(SocketCommandContext ctx)
        {
            var config = ServerConfig.Get(ctx.Guild);
            var adminRole = ctx.Guild.Roles.FirstOrDefault(r => r.Id == config.AdminRole);
            return adminRole != null && ((SocketGuildUser)ctx.User).Roles.Contains(adminRole);
        }
    }
}