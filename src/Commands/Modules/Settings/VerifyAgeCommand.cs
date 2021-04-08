using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public partial class SettingsModule
    {
        [Command("VerifyAge", "Va")]
        [Description(
            "Enables or disables the automatic account age warnings when a user who joins has a very young account (created within the last month).")]
        public Task<ActionResult> VerifyAgeAutomaticallyAsync(bool enabled)
        {
            Context.GuildData.Configuration.Moderation.CheckAccountAge = enabled;
            Db.Save(Context.GuildData);
            return Ok(enabled ? "Account age detection has been enabled." : "Account age detection has been disabled.");
        }
    }
}