using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Core.Commands.Preconditions
{
    public class RequireBotPermissionAttribute : CheckBaseAttribute
    {
        private readonly GuildPermission[] permissions;

        public RequireBotPermissionAttribute(GuildPermission[] perms)
        {
            permissions = perms;
        }

        public RequireBotPermissionAttribute(GuildPermission perm)
        {
            permissions = new[] {perm};
        }

        public override Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            foreach (var perm in ctx.Guild.CurrentUser.GuildPermissions.ToList())
                if (permissions.Contains(perm))
                    return Task.FromResult(CheckResult.Successful);
            return Task.FromResult(
                CheckResult.Unsuccessful("Bot is missing the required permssions to execute this command."));
        }
    }
}