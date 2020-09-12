using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Core.Entities
{
    public sealed class RequireBotChannelPermissionAttribute : CheckAttribute
    {
        private readonly Permissions _permission;

        public RequireBotChannelPermissionAttribute(Permissions permission) => _permission = permission;

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();
            var permissions = ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel);
            if (permissions.HasPermission(_permission))
                return CheckResult.Successful;
            if (ctx.Guild.CurrentMember.GetGuildPermissions().HasPermission(_permission))
                return CheckResult.Successful;

            await new DiscordEmbedBuilder()
                .AddField("Error in Command", ctx.Command.Name)
                .AddField("Error Reason", $"I am missing the following channel-level permissions required to execute this command: `{ _permission}`")
                .AddField("Correct Usage", ctx.Command.GetUsage(ctx))
                .WithAuthor(ctx.Member)
                .WithErrorColor()
                .SendToAsync(ctx.Channel);

            return CheckResult.Unsuccessful("Bot is missing the required permissions to execute this command.");
        }
    }
}