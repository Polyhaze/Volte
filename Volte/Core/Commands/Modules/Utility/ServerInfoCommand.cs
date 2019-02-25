using System.Threading.Tasks;
using Discord;
using Humanizer;
using Qmmands;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("ServerInfo", "Si")]
        [Description("Shows some info about the current guild.")]
        [Remarks("Usage: |prefix|serverinfo")]
        public async Task ServerInfoAsync()
        {
            var cAt = Context.Guild.CreatedAt;
            var embed = Context.CreateEmbedBuilder()
                .WithTitle("Server Info")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegionId)
                .AddField("Members", Context.Guild.Users.Count, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Voice Channels", Context.Guild.VoiceChannels.Count, true)
                .AddField("Text Channels", Context.Guild.TextChannels.Count, true);
            await embed.SendTo(Context.Channel);
        }
    }
}