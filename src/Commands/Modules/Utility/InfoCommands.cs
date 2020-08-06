using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("info")]
        public async Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author", $"{await Context.Client.Shards.First().Rest.GetUserAsync(168548441939509248)}, contributors on [GitHub](https://github.com/Ultz/Volte), and members of the Ultz organization.", true)
                .AddField("Language/Library", $"C# 8, Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Guilds", Context.Client.Guilds.Count, true)
                .AddField("Shards", Context.Client.Shards.Count, true)
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).Where(x => !(x is SocketCategoryChannel)).DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me", $"`{CommandService.GetCommand("Invite").GetUsage(Context)}`", true)
                .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
                .AddField("Successful Commands", CommandsService.SuccessfulCommandCalls, true)
                .AddField("Failed Commands", CommandsService.FailedCommandCalls, true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl()));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("userinfo [user]")]
        public Task<ActionResult> UserInfoAsync(SocketGuildUser user = null)
        {
            user ??= Context.User;

            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle("User Info")
                .AddField("User ID", user.Id, true)
                .AddField("Game", user.Activity?.Name ?? "Nothing", true)
                .AddField("Status", user.Status, true)
                .AddField("Is Bot", user.IsBot, true)
                .AddField("Account Created",
                    $"{user.CreatedAt.FormatDate()}, {user.CreatedAt.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{(user.JoinedAt.HasValue ? user.JoinedAt.Value.FormatDate() : "\u200B")}, " +
                    $"{(user.JoinedAt.HasValue ? user.JoinedAt.Value.FormatFullTime() : "\u200B")}")
                .WithThumbnailUrl(user.GetAvatarUrl(size: 512)));
        }

        [Command("GuildInfo", "Gi")]
        [Description("Shows some info about the current guild.")]
        [Remarks("guildinfo")]
        public Task<ActionResult> GuildInfoAsync()
        {
            var cAt = Context.Guild.CreatedAt;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Guild Info")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegionId)
                .AddField("Members", Context.Guild.Users.Count, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Category Channels", Context.Guild.CategoryChannels.Count, true)
                .AddField("Voice Channels", Context.Guild.VoiceChannels.Count, true)
                .AddField("Text Channels", Context.Guild.TextChannels.Count, true)
                .WithThumbnailUrl(Context.Guild.IconUrl));
        }

    }
}