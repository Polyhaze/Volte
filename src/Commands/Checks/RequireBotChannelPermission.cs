using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotChannelPermission : CheckBaseAttribute
    {
        private readonly ChannelPermission[] _permissions;

        public RequireBotChannelPermission(params ChannelPermission[] permissions) => _permissions = permissions;

        public override async Task<CheckResult> CheckAsync(
            ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).ToList())
            {
                if (ctx.Guild.CurrentUser.GuildPermissions.Administrator)
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            await new EmbedBuilder()
                .WithTitle("Error in Command")
                .AddField("Error Reason", $"I am missing the following channel-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .WithAuthor(ctx.User)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);

            return CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command.");
        }
    }
}