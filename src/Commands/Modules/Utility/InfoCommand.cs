using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Gommon;
using Qmmands;
using Volte.Extensions;

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
                .AddField("Language", $"C# - Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Servers", Context.Client.Guilds.Count, true)
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me", $"`{Db.GetConfig(Context.Guild).CommandPrefix}invite`", true)
                .AddField(".NET Core Version",
                    GetNetCoreVersion(out var ver) ? ver : "Couldn't fetch the version of .NET Core.", true)
                .AddField("Operating System", Environment.OSVersion.Platform, true)
                .AddField("Uptime", (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3), true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .SendToAsync(Context.Channel);
        }

        private bool GetNetCoreVersion(out string version)
        {
            Process process;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                {
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "/bin/bash",
                            Arguments = "-c \"dotnet --version\"",
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        }
                    };
                    process.Start();
                    version = process.StandardOutput.ReadToEnd();
                    return true;
                }

                case PlatformID.Win32NT:
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = "/C dotnet --version",
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        }
                    };
                    process.Start();
                    version = process.StandardOutput.ReadToEnd();
                    return true;
                default:
                    version = string.Empty;
                    return false;
            }
        }

        [Obsolete] //currently doesn't work properly in production or in debugging, put on the sidelines
        private string GetRamUsage()
        {
            return Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 3).ToString(CultureInfo.CurrentCulture);
        }
    }
}