using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("shutdown")]
        public Task<ActionResult> ShutdownAsync()
            => Ok($"Goodbye! {EmojiHelper.Wave}", _ =>
            {
                Cts.Cancel();
                return Task.CompletedTask;
            });
    }
}