using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("ServerPrefix")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("Usage: |prefix|serverprefix {newPrefix}")]
        [RequireGuildAdmin]
        public async Task ServerPrefixAsync([Remainder] string newPrefix)
        {
            var config = Db.GetConfig(Context.Guild);
            config.CommandPrefix = newPrefix;
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Set this server's prefix to **{newPrefix}**.").SendTo(Context.Channel);
        }
    }
}