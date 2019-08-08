using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotGuildPermissionAttribute : CheckBaseAttribute
    {
        private readonly GuildPermission[] _permissions;

        public RequireBotGuildPermissionAttribute(params GuildPermission[] perms)
        {
            _permissions = perms;
        }

        public override Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in ctx.Guild.CurrentUser.GuildPermissions.ToList())
            {
                if (ctx.Guild.CurrentUser.GuildPermissions.Administrator)
                    return Task.FromResult(CheckResult.Successful);
                if (_permissions.Contains(perm))
                    return Task.FromResult(CheckResult.Successful);
            }

            return Task.FromResult(
                CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command."));
        }
    }
}