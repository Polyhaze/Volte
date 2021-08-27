using System.Threading.Tasks;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class SettingsModule
    {
        [Command("ShowMod", "Srm")]
        [Description("Enables/Disables showing the moderator responsible for punishing users.")]
        public Task<ActionResult> ShowModeratorAsync(bool enabled)
        {
            Context.Modify(data => data.Configuration.Moderation.ShowResponsibleModerator = enabled);
            return Ok(enabled
                ? "Enabled showing the responsible moderator to users when they're punished."
                : "Disabled showing the responsible moderator to users when they're punished.");
        }
    }
}