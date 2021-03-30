using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class RequireBotChannelPermissionAttribute : CheckAttribute
    {
        private readonly ChannelPermission[] _permissions;

        public IEnumerable<ChannelPermission> Permissions => _permissions.ToList();

        public RequireBotChannelPermissionAttribute(params ChannelPermission[] permissions) => _permissions = permissions;

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            foreach (var perm in ctx.Guild.CurrentUser.GetPermissions(ctx.Channel).ToList())
            {
                if (ctx.Guild.CurrentUser.GuildPermissions.Administrator)
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            await ctx.CreateEmbedBuilder()
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following channel-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .SendToAsync(ctx.Channel);

            return CheckResult.Failed("Bot is missing the required permissions to execute this command.");
        }
    }
}