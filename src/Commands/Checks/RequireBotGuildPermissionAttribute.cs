using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotGuildPermissionAttribute : CheckAttribute
    {
        private readonly Permissions[] _permissions;

        public RequireBotGuildPermissionAttribute(params Permissions[] perms) => _permissions = perms;

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();
            var guildPermissions = ctx.Guild.CurrentMember.GetGuildPermissions();
            foreach (var perm in guildPermissions.GetFlags())
            {
                if (guildPermissions.HasPermission(Permissions.Administrator))
                    return CheckResult.Successful;
                if (_permissions.Contains(perm))
                    return CheckResult.Successful;
            }

            await new DiscordEmbedBuilder()
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following guild-level permissions required to execute this command: `{ _permissions.Select(x => x.ToString()).Join(", ")}`")
                .AddField("Correct Usage", ctx.Command.GetUsage(ctx))
                .WithAuthor(ctx.Member)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}