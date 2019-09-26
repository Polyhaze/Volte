using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotChannelPermissionAttribute : CheckAttribute
    {
        private readonly ChannelPermission[] _permissions;

        public RequireBotChannelPermissionAttribute(params ChannelPermission[] permissions) => _permissions = permissions;

        public override async ValueTask<CheckResult> CheckAsync(
            CommandContext context, IServiceProvider provider)
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
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following channel-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .AddField("Correct Usage", ctx.Command.GetUsage(ctx))
                .WithAuthor(ctx.User)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);

            return CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command.");
        }
    }
}