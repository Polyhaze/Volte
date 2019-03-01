using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("Usage: |prefix|antilink {true|false}")]
        [RequireGuildAdmin]
        public async Task AntilinkAsync(bool enabled)
        {
            var config = Db.GetConfig(Context.Guild);
            config.ModerationOptions.Antilink = enabled;
            Db.UpdateConfig(config);
            await Context.CreateEmbed(enabled ? "Antilink has been enabled." : "Antilink has been disabled.")
                .SendTo(Context.Channel);
        }
    }
}