using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.UserRewards {
    public class WhatLevelIsCommand : SivaCommand {
        [Command("WhatLevelIs"), Alias("Wli")]
        public async Task WhatLevelIs(uint xp) {
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"{xp.ToString()} XP is level {Math.Sqrt(xp / 50)}"));
        }
    }
}