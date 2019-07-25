﻿using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current server.")]
        [Remarks("Usage: |prefix|color {role}")]
        public Task<ActionResult> RoleColorAsync([Remainder] SocketRole role)
            => Ok($"**Dec:** {role.Color.RawValue}" +
                  "\n" +
                  $"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}");
    }
}