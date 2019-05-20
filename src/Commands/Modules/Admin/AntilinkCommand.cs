using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("Usage: |prefix|antilink {true|false}")]
        [RequireGuildAdmin]
        public async Task AntilinkAsync(bool enabled)
        {
            var data = Db.GetData(Context.Guild);
            config.ModerationOptions.Antilink = enabled;
            Db.UpdateData(config);
            await Context.CreateEmbed(enabled ? "Antilink has been enabled." : "Antilink has been disabled.")
                .SendToAsync(Context.Channel);
        }
    }
}