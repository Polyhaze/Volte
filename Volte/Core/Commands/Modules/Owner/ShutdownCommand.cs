using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("Shutdown")]
        [Summary("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        [RequireBotOwner]
        public async Task Shutdown() {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.ReactFailure();
                return;
            }

            await Context.CreateEmbed($"Goodbye! {RawEmoji.WAVE}").SendTo(Context.Channel);
            await VolteBot.Client.LogoutAsync();
            await VolteBot.Client.StopAsync();
            Environment.Exit(0);
        }
    }
}