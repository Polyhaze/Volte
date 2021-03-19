using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class AdminModule
    {
        [Command("ShowResponsibleModerator", "ShowMod", "Srm")]
        [Description("Enables/Disables showing the moderator responsible for punishing users.")]
        [Remarks("showresponsiblemoderator {Bool}")]
        [RequireGuildAdmin]
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