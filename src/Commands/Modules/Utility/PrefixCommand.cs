using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("Usage: |prefix|prefix")]
        public async Task PrefixAsync()
        {
            await Context.CreateEmbed($"The prefix for this server is **{Db.GetData(Context.Guild).Configuration.CommandPrefix}**.")
                .SendToAsync(Context.Channel);
        }
    }
}