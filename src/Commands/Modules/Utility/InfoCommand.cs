using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author",
                    "<@168548441939509248> and contributors on [GitHub](https://github.com/GreemDev/Volte)", true)
                //.AddField("RAM Usage", $"{GetRamUsage()}MB", true)
                .AddField("Language", $"C# - Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Servers", Context.Client.Guilds.Count, true)
                .AddField("Shards", Context.Client.Shards.Count, true)
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me", $"`{Db.GetData(Context.Guild).Configuration.CommandPrefix}invite`", true)
                .AddField(".NET Core Version", Environment.Version, true)
                .AddField("Operating System", Environment.OSVersion.Platform, true)
                .AddField("Uptime", (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3), true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl()));

        /*[Obsolete] //currently doesn't work properly in production or in debugging, put on the sidelines
        private string GetRamUsage()
        {
            return Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 3).ToString(CultureInfo.CurrentCulture);
        }*/
    }
}