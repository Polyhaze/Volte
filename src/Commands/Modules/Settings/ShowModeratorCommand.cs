using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public partial class SettingsModule
    {
        [Command("ShowResponsibleModerator", "ShowMod", "Srm")]
        [Description("Enables/Disables showing the moderator responsible for punishing users.")]
        public Task<ActionResult> ShowModeratorAsync(bool enabled)
        {
            Context.GuildData.Configuration.Moderation.ShowResponsibleModerator = enabled;
            Db.Save(Context.GuildData);
            return Ok(enabled
                ? "Enabled showing the responsible moderator to users when they're punished."
                : "Disabled showing the responsible moderator to users when they're punished.");
        }
    }
}