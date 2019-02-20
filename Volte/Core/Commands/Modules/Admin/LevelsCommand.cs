using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Levels")]
        [Description("Enables/Disables level gaining for this guild.")]
        [Remarks("Usage: $levels {true|false}")]
        [RequireGuildAdmin]
        public async Task LevelsAsync(bool arg)
        {
            var config = Db.GetConfig(Context.Guild);
            config.Leveling = arg;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(arg ? "Leveling has been enabled." : "Leveling has been disabled.")
                .SendTo(Context.Channel);
        }
    }
}