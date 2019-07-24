using System.Threading;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        public CancellationTokenSource Cts { get; set; }

        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public Task<VolteCommandResult> ShutdownAsync()
        {
            return Ok($"Goodbye! {EmojiService.Wave}", _ =>
            {
                Cts.Cancel();
                return Task.CompletedTask;
            });
        }
    }
}