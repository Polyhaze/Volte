using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("ReplyInline", "Ri")]
        [Description("Enable/Disable having commands reply inline.")]
        [Remarks("replyinline {Boolean}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ReplyInlineAsync(bool enabled)
        {
            Context.GuildData.Configuration.ReplyInline = enabled;
            Db.UpdateData(Context.GuildData);
            return Ok(enabled
                ? "Enabled ReplyInline in this guild."
                : "Disabled ReplyInline in this guild.");
        }
    }
}