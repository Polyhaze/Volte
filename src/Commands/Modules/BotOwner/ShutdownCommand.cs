using System.Threading;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        public CancellationTokenSource Cts { get; set; }

        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("shutdown")]
        [RequireBotOwner]
        public Task<ActionResult> ShutdownAsync()
            => Ok($"Goodbye! {EmojiService.Wave}", _ =>
            {
                Cts.Cancel();
                return Task.CompletedTask;
            });
    }
}