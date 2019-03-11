using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        public CancellationTokenSource Cts { get; set; }

        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public async Task ShutdownAsync()
        {
            await Context.CreateEmbed($"Goodbye! {EmojiService.WAVE}").SendToAsync(Context.Channel);
            await Context.Client.SetStatusAsync(UserStatus.Invisible);
            await Context.Client.LogoutAsync();
            await Context.Client.StopAsync();
            Cts.Cancel();
            Context.Client.Dispose();
            Environment.Exit(0);
        }
    }
}