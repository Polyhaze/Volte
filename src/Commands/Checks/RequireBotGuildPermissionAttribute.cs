using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotGuildPermissionAttribute : CheckBaseAttribute
    {
        private readonly GuildPermission[] _permissions;

        public RequireBotGuildPermissionAttribute(params GuildPermission[] perms) => _permissions = perms;

        public override async Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in ctx.Guild.CurrentUser.GuildPermissions.ToList())
            {
                if (ctx.Guild.CurrentUser.GuildPermissions.Administrator)
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            await new EmbedBuilder()
                .WithTitle("Error in Command")
                .AddField("Error Reason", $"I am missing the following server-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .WithAuthor(ctx.User)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}