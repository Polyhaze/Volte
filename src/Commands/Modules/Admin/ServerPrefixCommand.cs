using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("ServerPrefix")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("Usage: |prefix|serverprefix {newPrefix}")]
        [RequireGuildAdmin]
        public async Task ServerPrefixAsync([Remainder] string newPrefix)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.CommandPrefix = newPrefix;
            Db.UpdateData(data);
            await Context.CreateEmbed($"Set this server's prefix to **{newPrefix}**.").SendToAsync(Context.Channel);
        }
    }
}