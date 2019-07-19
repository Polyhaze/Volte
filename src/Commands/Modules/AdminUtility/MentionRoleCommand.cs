using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules.AdminUtility
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("Usage: |prefix|mentionrole {role}")]
        [RequireBotGuildPermission(GuildPermission.ManageRoles | GuildPermission.Administrator)]
        [RequireGuildAdmin]
        public async Task<BaseResult> MentionRoleAsync([Remainder] SocketRole role)
        {
            if (role.IsMentionable)
            {
                return Ok(role.Mention, null, false);
            }

            await role.ModifyAsync(x => x.Mentionable = true);
            await Context.ReplyAsync(role.Mention);
            return Ok(role.Mention, async m => await role.ModifyAsync(x => x.Mentionable = false));
        }
    }
}