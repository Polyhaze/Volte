using System.Threading.Tasks;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class SettingsModule
    {
        [Command("AutoQuote", "QuoteLinkReply", "QuoteLink", "QuoteReply", "JumpUrlReply", "Qrl", "Qlr")]
        [Description(
            "Enables or disables the Quote link parsing and sending into a channel that a 'Quote URL' is posted to for this guild.")]
        public Task<ActionResult> QuoteLinkReplyCommandAsync(bool enabled)
        {
            Context.Modify(data => data.Extras.AutoParseQuoteUrls = enabled);
            return Ok(enabled ? "Enabled QuoteLinkReply for this guild." : "Disabled QuoteLinkReply for this guild.");
        }
    }
}