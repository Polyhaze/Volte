using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Preconditions
{
    public sealed class RequireBotGuildPermissionAttribute : CheckBaseAttribute
    {
        private readonly GuildPermission[] _permissions;

        public RequireBotGuildPermissionAttribute(params GuildPermission[] perms) => _permissions = perms;

        public override async Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var currentUser = await ctx.Guild.GetCurrentUserAsync();
            foreach (var perm in currentUser.GuildPermissions.ToList())
            {
                if (currentUser.GuildPermissions.Administrator)
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            return CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command.");
        }
    }
}