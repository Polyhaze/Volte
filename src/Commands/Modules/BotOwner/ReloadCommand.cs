using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Core;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("Reload", "Rl")]
        [Description(
            "Reloads the bot's configuration file if you've changed it. NOTE: This will throw an exception if the config file is invalid JSON!")]
        [Remarks("reload")]
        [RequireBotOwner]
        public Task ReloadAsync()
            => Config.Reload(Context.ServiceProvider)
                ? Ok("Config reloaded!")
                : BadRequest("Something bad happened. Check console for more detailed information.");
    }
}