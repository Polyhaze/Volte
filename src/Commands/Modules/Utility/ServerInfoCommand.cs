using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Data.Models.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("ServerInfo", "Si")]
        [Description("Shows some info about the current guild.")]
        [Remarks("Usage: |prefix|serverinfo")]
        public async Task<VolteCommandResult> ServerInfoAsync()
        {
            var cAt = Context.Guild.CreatedAt;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Server Info")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegionId)
                .AddField("Members", (await Context.Guild.GetUsersAsync()).Count, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Voice Channels", (await Context.Guild.GetVoiceChannelsAsync()).Count, true)
                .AddField("Text Channels", (await Context.Guild.GetTextChannelsAsync()).Count, true));
        }
    }
}