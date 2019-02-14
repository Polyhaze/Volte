using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Economy
{
    public partial class EconomyModule : VolteModule
    {
        [Command("WhatLevelIs", "Wli")]
        [Description("Checks what level the given amount of XP is equal to.")]
        [Remarks("Usage: |prefix|whatlevelis {xpAmount}")]
        public async Task WhatLevelIs(uint xp)
        {
            // ReSharper disable once PossibleLossOfFraction (we don't care about loss of fraction)
            await Context.CreateEmbed($"{xp} XP is level {Math.Sqrt(xp / 50)}").SendTo(Context.Channel);
        }
    }
}