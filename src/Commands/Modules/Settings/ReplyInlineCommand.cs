using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("ReplyInline", "Ri")]
        [Description("Enable/Disable having commands reply inline.")]
        public Task<ActionResult> ReplyInlineAsync(bool enabled)
        {
            Context.GuildData.Configuration.ReplyInline = enabled;
            Db.Save(Context.GuildData);
            return Ok(enabled
                ? "Enabled ReplyInline in this guild."
                : "Disabled ReplyInline in this guild.");
        }
    }
}