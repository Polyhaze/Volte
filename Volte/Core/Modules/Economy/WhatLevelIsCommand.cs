using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("WhatLevelIs"), Alias("Wli")]
        [Summary("Checks what level the given amount of XP is equal to.")]
        [Remarks("Usage: |prefix|whatlevelis {xpAmount}")]
        public async Task WhatLevelIs(uint xp) {
            await Reply(Context.Channel,
                // ReSharper disable once PossibleLossOfFraction (we don't care about loss of fraction)
                CreateEmbed(Context, $"{xp} XP is level {Math.Sqrt(xp / 50)}"));
        }
    }
}