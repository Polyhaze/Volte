using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {role}")]
        [RequireGuildAdmin]
        public async Task ModRoleAsync(SocketRole role)
        {
            var config = Db.GetConfig(Context.Guild);
            if (role != null)
            {
                config.ModerationOptions.ModRole = role.Id;
                Db.UpdateConfig(config);
                await Context.CreateEmbed($"Set **{role.Name}** as the Moderator role for this server.")
                    .SendTo(Context.Channel);
            }
        }
    }
}