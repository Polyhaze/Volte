using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Humanizer;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.General
{
    public partial class GeneralModule : VolteModule
    {
        [Command("Uptime")]
        [Description("Shows the bot's uptime in a human-friendly fashion.")]
        [Remarks("Usage: |prefix|uptime")]
        public async Task UptimeAsync()
        {
            var time = (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3);
            await Context.CreateEmbed($"I've been online for **{time}**!").SendTo(Context.Channel);
        }
    }
}