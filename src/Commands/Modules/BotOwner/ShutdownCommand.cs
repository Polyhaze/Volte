using System.Threading.Tasks;
using Qmmands;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        public Task<ActionResult> ShutdownAsync()
            => Ok($"Goodbye! {DiscordHelper.Wave}", async _ =>
            {
                await Task.Yield();
                Cts.Cancel();
            });
    }
}