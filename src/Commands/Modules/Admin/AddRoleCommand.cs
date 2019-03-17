using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("Usage: |prefix|addrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task AddRoleAsync(DiscordMember user, [Remainder] DiscordRole role)
        {
            await user.GrantRoleAsync(role);
            await Context.CreateEmbed($"Added the role **{role}** to {user.Mention}!")
                .SendToAsync(Context.Channel);
        }
    }
}