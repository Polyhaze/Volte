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
        private readonly Permissions _permission;

        public RequireBotGuildPermissionAttribute(Permissions perm) => _permission = perm;

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();
            var guildPermissions = ctx.Guild.CurrentMember.GetGuildPermissions();
            if (guildPermissions.HasPermission(Permissions.Administrator))
                return CheckResult.Successful;
            if (guildPermissions.GetFlags().Any(perm => _permission == perm))
            {
                return CheckResult.Successful;
            }

            await new DiscordEmbedBuilder()
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following guild-level permissions required to execute this command: `{ _permission}`")
                .AddField("Correct Usage", ctx.Command.GetUsage(ctx))
                .WithAuthor(ctx.Member)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}