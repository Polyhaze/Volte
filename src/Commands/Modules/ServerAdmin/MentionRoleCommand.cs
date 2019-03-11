using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;

namespace Volte.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("MentionRole", "Menro", "Mr")]
        [Description("Mentions a role. If it isn't mentionable, it allows it to be, mentions it, and then undoes the first action.")]
        [Remarks("Usage: |prefix|mentionrole {role}")]
        [RequireGuildAdmin]
        public async Task MentionRoleAsync([Remainder] SocketRole role)
        {
            if (role.IsMentionable)
            {
                await Context.ReplyAsync(role.Mention);
            }
            else
            {
                await role.ModifyAsync(x => x.Mentionable = true);
                await Context.ReplyAsync(role.Mention);
                await role.ModifyAsync(x => x.Mentionable = false);
            }
        }
    }
}
