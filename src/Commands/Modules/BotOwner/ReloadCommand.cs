using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("Reload", "Rl")]
        [Description(
            "Reloads the bot's configuration file if you've changed it. NOTE: This will throw an exception if the config file is invalid JSON!")]
        [Remarks("Usage: |prefix|reload")]
        [RequireBotOwner]
        public Task ReloadAsync()
            => Config.Reload(Context.ServiceProvider)
                ? Ok("Config reloaded!")
                : BadRequest("Something bad happened. Check console for more detailed information.");
    }
}