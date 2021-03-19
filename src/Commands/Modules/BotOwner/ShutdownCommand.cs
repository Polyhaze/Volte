using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("shutdown")]
        [RequireBotOwner]
        public Task<ActionResult> ShutdownAsync()
            => Ok($"Goodbye! {DiscordHelper.Wave}", _ =>
            {
                Cts.Cancel();
                return Task.CompletedTask;
            });
    }
}