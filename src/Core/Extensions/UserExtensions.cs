using System.Linq;
using Discord;
using Volte.Core.Commands;
using Volte.Core.Data;
using Volte.Core.Discord;
using Volte.Core.Services;

namespace Volte.Core.Extensions
{
    public static class UserExtensions
    {
        private static DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();

        public static bool IsBotOwner(this IGuildUser user)
            => Config.Owner.Equals(user.Id);

        public static bool IsGuildOwner(this IGuildUser user)
            => user.Guild.OwnerId.Equals(user.Id) || IsBotOwner(user);

        public static bool HasRole(this IGuildUser user, IRole role)
            => HasRole(user, role.Id);

        public static bool IsModerator(this IGuildUser user)
            => HasRole(user, _db.GetConfig(user.Guild.Id).ModerationOptions.ModRole) ||
               IsAdmin(user) ||
               IsGuildOwner(user);

        public static bool HasRole(this IGuildUser user, ulong roleId)
            => user.RoleIds.Contains(roleId);

        public static bool IsAdmin(this IGuildUser user)
            => HasRole(user, _db.GetConfig(user.Guild).ModerationOptions.AdminRole) ||
               IsGuildOwner(user);
    }
}