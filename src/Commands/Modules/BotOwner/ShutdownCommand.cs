using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Core;
using Volte.Extensions;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public async Task ShutdownAsync()
        {
            await Context.CreateEmbed($"Goodbye! {EmojiService.WAVE}").SendToAsync(Context.Channel);
            VolteBot.Cts.Cancel();
        }
    }
}