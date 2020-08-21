using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description(
            "Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("mentionrole {Role}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public Task<ActionResult> MentionRoleAsync([Remainder]DiscordRole role)
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