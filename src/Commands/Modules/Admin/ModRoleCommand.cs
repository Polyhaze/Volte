using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
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
            config.ModerationOptions.ModRole = role.Id;
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Set **{role.Name}** as the Moderator role for this server.")
                .SendToAsync(Context.Channel);
        }
    }
}