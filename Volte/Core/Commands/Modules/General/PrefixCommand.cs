using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.General
{
    public partial class GeneralModule : VolteModule
    {
        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("Usage: |prefix|prefix")]
        public async Task Prefix()
        {
            await Context.CreateEmbed($"The prefix for this server is **{Db.GetConfig(Context.Guild).CommandPrefix}**.")
                .SendTo(Context.Channel);
        }
    }
}