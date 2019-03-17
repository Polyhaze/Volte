using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Preconditions
{
    public class RequireBotGuildPermissionAttribute : CheckBaseAttribute
    {
        private readonly Permissions[] _permissions;

        public RequireBotGuildPermissionAttribute(params Permissions[] perms) => _permissions = perms;


        public override Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            return Task.FromResult(
                _permissions.Any(perm => ctx.Guild.CurrentMember.Roles.Any(x => x.Permissions.HasPermission(perm)))
                    ? CheckResult.Successful
                    : CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command."));
        }
    }
}