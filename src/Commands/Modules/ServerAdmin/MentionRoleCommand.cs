using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Preconditions;

namespace Volte.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("Usage: |prefix|mentionrole {role}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        [RequireGuildAdmin]
        public async Task MentionRoleAsync([Remainder] DiscordRole role)
        {
            if (role.IsMentionable)
            {
                await Context.ReplyAsync(role.Mention);
            }
            else
            {
                await role.ModifyAsync(r => r.Mentionable = true);
                await Context.ReplyAsync(role.Mention);
                await role.ModifyAsync(r => r.Mentionable = false);
            }
        }
    }
}
