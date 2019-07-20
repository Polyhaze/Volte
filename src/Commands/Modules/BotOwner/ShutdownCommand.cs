using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Core;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public Task<VolteCommandResult> ShutdownAsync()
        {
            return Ok($"Goodbye! {EmojiService.WAVE}", _ =>
            {
                VolteBot.Cts.Cancel();
                return Task.CompletedTask;
            });
        }
    }
}