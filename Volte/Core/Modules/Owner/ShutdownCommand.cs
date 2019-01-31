﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        // I'm not sure how well this works.

        [Command("Shutdown")]
        [Summary("Forces the bot to shutdown.")]
        [Remarks("Usage: |prefix|shutdown")]
        public async Task Shutdown() {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await Reply(Context.Channel, CreateEmbed(Context, $"Goodbye! {RawEmoji.WAVE}"));
            await VolteBot.Client.LogoutAsync();
            await VolteBot.Client.StopAsync();
            Environment.Exit(0);
        }
    }
}