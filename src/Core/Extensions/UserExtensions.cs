using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using BrackeysBot.Commands;

namespace BrackeysBot
{
    public static class UserExtensions
    {
        public static async Task MuteAsync(this IGuildUser user, ICommandContext context)
            => await user.AddRoleAsync(GetMutedRole(context));
        public static async Task UnmuteAsync(this IGuildUser user, ICommandContext context)
            => await user.RemoveRoleAsync(GetMutedRole(context));

        private static IRole GetMutedRole(ICommandContext context)
            => context.Guild.GetRole((context as BrackeysBotContext).Configuration.MutedRoleID);

        public static string Mention(this ulong userId) 
            => $"<@{userId}>";
        public static string EnsureAvatarUrl(this IUser user)
            => user.GetAvatarUrl().WithAlternative(user.GetDefaultAvatarUrl());

        public static PermissionLevel GetPermissionLevel(this IGuildUser user, ICommandContext context)
            => (context is BrackeysBotContext botContext) 
                ? GetPermissionLevel(user, botContext.Configuration)
                : PermissionLevel.Default;

        public static PermissionLevel GetPermissionLevel(this IGuildUser user, BotConfiguration config) 
        {
            if (user.GuildPermissions.Has(GuildPermission.Administrator))
                return PermissionLevel.Administrator;
            if (user.RoleIds.Contains(config.ModeratorRoleID))
                return PermissionLevel.Moderator;
            if (user.RoleIds.Contains(config.GuruRoleID))
                return PermissionLevel.Guru;

            return PermissionLevel.Default;

        }
    }
}
