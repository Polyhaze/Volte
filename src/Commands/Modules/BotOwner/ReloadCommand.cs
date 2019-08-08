using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("Reload", "Rl")]
        [Description(
            "Reloads the bot's configuration file if you've changed it. NOTE: This will throw an exception if the config file is invalid JSON!")]
        [Remarks("Usage: |prefix|reload")]
        [RequireBotOwner]
        public Task ReloadAsync()
        {
            return Config.Reload(Context.ServiceProvider)
                ? Ok("Config reloaded!")
                : BadRequest("Something bad happened. Check console for more detailed information.");
        }
    }
}