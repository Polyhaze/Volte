using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("WhatLevelIs"), Alias("Wli")]
        public async Task WhatLevelIs(uint xp) {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"{xp.ToString()} XP is level {Math.Sqrt(xp / 50)}"));
        }
    }
}