using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("MentionRole", "Menro")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("mentionrole {Role}")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles)]
        [RequireGuildAdmin]
        public Task<ActionResult> MentionRoleAsync([Remainder] SocketRole role)
        {
            if (role.IsMentionable)
            {
                return Ok(role.Mention, shouldEmbed: false);
            }

            return Ok(async () =>
            {
                await role.ModifyAsync(x => x.Mentionable = true);
                await Context.ReplyAsync(role.Mention);
                await role.ModifyAsync(x => x.Mentionable = false);
            });
        }
    }
}