using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

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
                .AddField("Author", $"{await Context.Client.ShardClients.First().Value.GetUserAsync(168548441939509248)}, contributors on [GitHub](https://github.com/Ultz/Volte), and members of the Ultz organization.", true)
                .AddField("Language/Library", $"C# 8, Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Guilds", Context.Client.GetGuildCount(), true)
                .AddField("Shards", Context.Client.ShardClients.Count, true)
                .AddField("Channels", Context.Client.GetChannelCount(), true) // TODO grossly oversimplified for now
                .AddField("Invite Me", $"`{CommandService.GetCommand("Invite").GetUsage(Context)}`", true)
                .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
                .AddField("Successful Commands", CommandsService.SuccessfulCommandCalls, true)
                .AddField("Failed Commands", CommandsService.FailedCommandCalls, true)
                .WithThumbnail(Context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Auto, 512)));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("userinfo [user]")]
        public Task<ActionResult> UserInfoAsync(DiscordMember user = null)
        {
            user ??= Context.Member;

            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto, 512))
                .WithTitle("User Info")
                .AddField("User ID", user.Id, true)
                .AddField("Game", user.Activity?.Name ?? "Nothing", true) // TODO: DSharpPlus does not cache activity.
                .AddField("Status", user.Status, true) // TODO: DSharpPlus does not cache activity.
                .AddField("Is Bot", user.IsBot, true)
                .AddField("Account Created",
                    $"{user.CreationTimestamp.FormatDate()}, {user.CreationTimestamp.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{(user.JoinedAt != default ? user.JoinedAt.FormatDate() : "\u200B")}, " +
                    $"{(user.JoinedAt != default ? user.JoinedAt.FormatFullTime() : "\u200B")}")
                .WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto, 512)));
        }

        [Command("GuildInfo", "Gi")]
        [Description("Shows some info about the current guild.")]
        [Remarks("guildinfo")]
        public Task<ActionResult> GuildInfoAsync()
        {
            var cAt = Context.Guild.CreationTimestamp;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Guild Info")
                .WithThumbnail(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegion.Id)
                .AddField("Members", Context.Guild.MemberCount, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Category Channels", Context.Guild.GetCategoryChannels().Count(), true)
                .AddField("Voice Channels", Context.Guild.GetVoiceChannels().Count(), true)
                .AddField("Text Channels", Context.Guild.GetTextChannels().Count(), true)
                .WithThumbnail(Context.Guild.IconUrl));
        }

    }
}