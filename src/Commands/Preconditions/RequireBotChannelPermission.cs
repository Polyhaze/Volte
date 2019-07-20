using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Preconditions
{
    public sealed class RequireBotChannelPermission : CheckBaseAttribute
    {
        private readonly ChannelPermission[] _permissions;

        public RequireBotChannelPermission(params ChannelPermission[] permissions) => _permissions = permissions;

        public override async Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            var currentUser = await ctx.Guild.GetCurrentUserAsync();
            foreach (var perm in currentUser.GetPermissions(ctx.Channel).ToList())
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