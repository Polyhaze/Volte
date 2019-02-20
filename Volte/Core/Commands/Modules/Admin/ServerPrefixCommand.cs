using System.Threading.Tasks;
using Discord;
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
        public async Task ServerPrefixAsync([Remainder] string prefix)
        {
            var config = Db.GetConfig(Context.Guild);
            config.CommandPrefix = prefix;
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Set this server's prefix to **{prefix}**.").SendTo(Context.Channel);
        }
    }
}