using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("Usage: |prefix|adminrole {role}")]
        [RequireGuildAdmin]
        public Task<BaseResult> AdminRoleAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.AdminRole = role.Id;
            Db.UpdateData(data);
            return Ok($"Set **{role.Name}** as the Admin role for this server.");
        }
    }
}