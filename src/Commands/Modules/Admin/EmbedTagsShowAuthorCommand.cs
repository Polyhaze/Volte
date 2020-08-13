using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule
    {
        [Command("TagShow", "TagSh")]
        [Description("Toggles whether or not Tags requested in your guild will be in an embed and be shown with the person who requested the Tag.")]
        [Remarks("tagshow {Boolean}")]   
        public Task<ActionResult> ShowRequesterAndEmbedTagsAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.EmbedTagsAndShowAuthor = enabled; 
                return data;
            });
            return Ok(enabled
                ? "Tags will now show their requester and be displayed in an embed!"
                : "Tags will **NO LONGER** show their requester and be displayed in an embed!");
        }

    }
}
