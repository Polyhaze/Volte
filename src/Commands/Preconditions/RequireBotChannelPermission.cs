using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Preconditions
{
    public class RequireBotChannelPermission : CheckBaseAttribute
    {
        private readonly Permissions[] _permissions;

        public RequireBotChannelPermission(params Permissions[] permissions) => _permissions = permissions;


        public override Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            return Task.FromResult(
                _permissions.Any(perm => ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(perm))
                    ? CheckResult.Successful
                    : CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command."));
        }
    }
}