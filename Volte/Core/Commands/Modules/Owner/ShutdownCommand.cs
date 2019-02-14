using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("Shutdown")]
        [Description("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public async Task Shutdown() {
            await Context.CreateEmbed($"Goodbye! {RawEmoji.WAVE}").SendTo(Context.Channel);
            await VolteBot.Client.LogoutAsync();
            await VolteBot.Client.StopAsync();
            Environment.Exit(0);
        }
    }
}