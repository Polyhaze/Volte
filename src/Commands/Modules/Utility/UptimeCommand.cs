using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Discord.Commands;

using Humanizer;
using Humanizer.Localisation;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        [Command("uptime")]
        [Summary("Displays the time the bot has been running for.")]
        public async Task DisplayUptimeAsync()
        {
            TimeSpan uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

            await ReplyAsync($"I've been up and running for {uptime.Humanize(precision: 3, minUnit: TimeUnit.Second)}.");
        }
    }
}
