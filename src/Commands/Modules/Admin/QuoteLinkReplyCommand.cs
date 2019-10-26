using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("QuoteLinkReply", "QuoteLink", "QuoteReply", "JumpUrlReply", "Qrl")]
        [Description("Enables or disables the Quote link parsing and sending into a channel that a 'Quote URL' is posted to for this guild.")]
        [Remarks("quotelinkreply {true/false}")]
        public Task<ActionResult> QuoteLinkReplyCommandAsync(bool enabled)
        {
            Context.GuildData.Extras.AutoParseQuoteUrls = enabled;
            Db.UpdateData(Context.GuildData);
            return Ok(enabled ? "Enabled QuoteLinkReply for this guild." : "Disabled QuoteLinkReply for this guild.");
        }   
    }
}