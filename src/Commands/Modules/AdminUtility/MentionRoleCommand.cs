using System.Threading.Tasks;
using Discord;
 
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule : VolteModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("Usage: |prefix|mentionrole {role}")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles)]
        [RequireGuildAdmin]
        public async Task<ActionResult> MentionRoleAsync([Remainder] SocketRole role)
        {
            if (role.IsMentionable) return Ok(role.Mention, null, false);

            await role.ModifyAsync(x => x.Mentionable = true);
            return Ok(role.Mention, async _ => await role.ModifyAsync(x => x.Mentionable = false), false);
        }
    }
}