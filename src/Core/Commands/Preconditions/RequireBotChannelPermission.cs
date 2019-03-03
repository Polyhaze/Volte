using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;

namespace Volte.Core.Commands.Preconditions
{
    public class RequireBotChannelPermission : CheckBaseAttribute
    {
        private readonly ChannelPermission[] _permissions;

        public RequireBotChannelPermission(ChannelPermission[] permissions) => _permissions = permissions;

        public RequireBotChannelPermission(ChannelPermission permission) => _permissions = new[] {permission};


        public override Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            foreach (var perm in ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).ToList())
                if (_permissions.Contains(perm))
                    return Task.FromResult(CheckResult.Successful);
            return Task.FromResult(
                CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command."));
        }
    }
}