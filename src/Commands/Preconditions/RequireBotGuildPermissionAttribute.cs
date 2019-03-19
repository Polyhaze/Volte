using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Preconditions
{
    public class RequireBotGuildPermissionAttribute : CheckBaseAttribute
    {
        private readonly GuildPermission[] _permissions;

        public RequireBotGuildPermissionAttribute(params GuildPermission[] perms) => _permissions = perms;


        public override async Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in (await ctx.Guild.GetCurrentUserAsync()).GuildPermissions.ToList())
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            return CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command.");
        }
    }
}