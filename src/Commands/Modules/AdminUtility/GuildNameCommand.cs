﻿using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("GuildName", "Gn")]
        [Description("Sets the name of the current guild.")]
        public async Task<ActionResult> GuildNameAsync([Remainder, Description("The name to give this guild.")]
            string name)
        {
            await Context.Guild.ModifyAsync(g => g.Name = name);
            return Ok($"Set this guild's name to **{name}**!");
        }
    }
}