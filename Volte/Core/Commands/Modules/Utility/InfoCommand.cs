using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Core.Extensions;
using Version = Volte.Core.Runtime.Version;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public async Task InfoAsync()
        {
            await Context.CreateEmbedBuilder(string.Empty)
                .AddField("Version", Version.FullVersion)
                .AddField("Author", "<@168548441939509248>")
                .AddField("Language", "C# - Discord.Net 2.0.1")
                .AddField("Server Count", Context.Client.Guilds.Count)
                .AddField("Invite Me", $"`{Db.GetConfig(Context.Guild).CommandPrefix}invite`")
                .AddField(".NET Core Version", GetNetCoreVersion())
                .AddField("Operating System", Environment.OSVersion)
                .AddField("Uptime", (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3))
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .SendTo(Context.Channel);
        }

        private string GetNetCoreVersion()
        {
            //looks ghetto, but this is the only way i'm aware of to get the current .NET core version, since the runtime has no other blatant methods.
            //feel free to open a PR to make this not nasty.
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(
                    AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "System.Private.CoreLib")
                        .Location), ".version")
            ).Split('\n')[1];
        }
    }
}