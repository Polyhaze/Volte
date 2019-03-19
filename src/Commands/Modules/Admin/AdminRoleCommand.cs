using Discord.WebSocket;
using Qmmands;
using System.Threading.Tasks;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("Usage: |prefix|adminrole {role}")]
        [RequireGuildAdmin]
        public async Task AdminRoleAsync(SocketRole role)
        {
            var embed = Context.CreateEmbedBuilder();
            var config = Db.GetConfig(Context.Guild);
            config.ModerationOptions.AdminRole = role.Id;
            Db.UpdateConfig(config);
            embed.WithDescription($"Set **{role.Name}** as the Admin role for this server.");
            await embed.SendToAsync(Context.Channel);
        }
    }
}