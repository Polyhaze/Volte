using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("ServerInfo", "Si")]
        [Description("Shows some info about the current guild.")]
        [Remarks("Usage: |prefix|serverinfo")]
        public async Task ServerInfoAsync()
        {
            var cAt = Context.Guild.CreationTimestamp;
            var embed = Context.CreateEmbedBuilder()
                .WithTitle("Server Info")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegion.Id)
                .AddField("Members", Context.Guild.MemberCount.ToString(), true)
                .AddField("Roles", Context.Guild.Roles.Count.ToString(), true)
                .AddField("Voice Channels", Context.Guild.Channels.Count.ToString(), true);
            await embed.SendToAsync(Context.Channel);
        }
    }
}