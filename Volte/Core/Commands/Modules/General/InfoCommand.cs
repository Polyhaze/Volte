using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Core.Extensions;
using Version = Volte.Core.Runtime.Version;

namespace Volte.Core.Commands.Modules.General
{
    public partial class GeneralModule : VolteModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public async Task InfoAsync()
        {
            await Context.CreateEmbedBuilder(string.Empty)
                .AddField("Version", Version.GetFullVersion())
                .AddField("Author", "<@168548441939509248>")
                .AddField("Language", "C# - Discord.Net 2.0.1")
                .AddField("Server Count", Context.Client.Guilds.Count)
                .AddField("Invite Me", $"`{Db.GetConfig(Context.Guild).CommandPrefix}invite`")
                .AddField("Operating System", Environment.OSVersion)
                .AddField("Uptime", (DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3))
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .SendTo(Context.Channel);
        }
    }
}