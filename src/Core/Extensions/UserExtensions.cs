using System;
using System.Linq;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static bool IsBotOwner(this IGuildUser user) => Config.Owner.Equals(user.Id);

        public static bool IsGuildOwner(this IGuildUser user)
            => user.Guild.OwnerId.Equals(user.Id) || IsBotOwner(user);

        public static bool IsModerator(this IGuildUser user, IServiceProvider provider)
            => HasRole(user,
                   provider.GetRequiredService<DatabaseService>().GetData(user.Guild).Configuration.Moderation
                       .ModRole) ||
               IsAdmin(user, provider) ||
               IsGuildOwner(user);

        public static bool HasRole(this IGuildUser user, ulong roleId) => user.RoleIds.Contains(roleId);

        public static bool IsAdmin(this IGuildUser user, IServiceProvider provider)
            => HasRole(user,
                   provider.GetRequiredService<DatabaseService>().GetData(user.Guild).Configuration.Moderation
                       .AdminRole) ||
               IsGuildOwner(user);
    }
}