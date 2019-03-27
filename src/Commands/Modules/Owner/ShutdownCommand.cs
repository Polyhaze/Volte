using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
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