using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Humanizer;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("ServerInfo", "Si")]
        [Description("Shows some info about the current guild.")]
        [Remarks("Usage: |prefix|serverinfo")]
        public async Task ServerInfo() {
            var cAt = Context.Guild.CreatedAt;
            var embed = new EmbedBuilder()
                .WithTitle("Server Info")
                .WithAuthor(Context.User)
                .WithColor(Config.GetSuccessColor())
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