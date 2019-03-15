using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Extensions;
using Version = Volte.Version;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public async Task InfoAsync()
        {
            await Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author",
                    "<@168548441939509248> and contributors on [GitHub](https://github.com/GreemDev/Volte)", true)
                //.AddField("RAM Usage", $"{GetRamUsage()}MB", true)
                .AddField("Language", "C# - Discord.Net 2.0.1", true)
                .AddField("Servers", Context.Client.Guilds.Count, true)
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me", $"`{Db.GetConfig(Context.Guild).CommandPrefix}invite`", true)
                .AddField(".NET Core Version", GetDotNetCoreVersion(), true)
                .AddField("Operating System", Environment.OSVersion.Platform, true)
                .AddField("Uptime", (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3), true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .SendToAsync(Context.Channel);
        }

        private string GetDotNetCoreVersion()
        {
            //looks ghetto, but this is the only way i'm aware of to get the current .NET core version, since the runtime has no other blatant methods.
            //feel free to open a PR to make this not nasty.
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(
                    AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "System.Private.CoreLib")
                        .Location), ".version")
            ).Split('\n')[1];
        }

        [Obsolete] //currently doesn't work properly in production or in debugging, put on the sidelines
        private string GetRamUsage()
        {
            return Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 3).ToString(CultureInfo.CurrentCulture);
        }
    }
}