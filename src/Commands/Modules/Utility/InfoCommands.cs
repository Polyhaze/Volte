using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        [Remarks("Usage: |prefix|info")]
        public Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author",
                    "<@168548441939509248> and contributors on [GitHub](https://github.com/GreemDev/Volte)", true)
                .AddField("Language", $"C# - Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Servers", Context.Client.Guilds.Count, true)
                .AddField("Shards", Context.Client.Shards.Count, true)
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me", $"`{Db.GetData(Context.Guild).Configuration.CommandPrefix}invite`", true)
                .AddField(".NET Core Version", Environment.Version, true)
                .AddField("Operating System", Environment.OSVersion.Platform, true)
                .AddField("Uptime", GetUptime(), true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl()));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        [Remarks("Usage: |prefix|userinfo [user]")]
        public Task<ActionResult> UserInfoAsync(SocketGuildUser user = null)
        {
            var target = user ?? Context.User;

            return Ok(Context.CreateEmbedBuilder()
                .WithThumbnailUrl(target.GetAvatarUrl())
                .WithTitle("User Info")
                .AddField("User ID", target.Id, true)
                .AddField("Game", target.Activity?.Name ?? "Nothing", true)
                .AddField("Status", target.Status, true)
                .AddField("Is Bot", target.IsBot, true)
                .AddField("Account Created",
                    $"{target.CreatedAt.FormatDate()}, {target.CreatedAt.FormatFullTime()}")
                .AddField("Joined This Guild",
                    $"{(target.JoinedAt.HasValue ? target.JoinedAt.Value.FormatDate() : "\u200B")}, " +
                    $"{(target.JoinedAt.HasValue ? target.JoinedAt.Value.FormatFullTime() : "\u200B")}"));
        }

        [Command("ServerInfo", "Si")]
        [Description("Shows some info about the current guild.")]
        [Remarks("Usage: |prefix|serverinfo")]
        public Task<ActionResult> ServerInfoAsync()
        {
            var cAt = Context.Guild.CreatedAt;

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle("Server Info")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Name", Context.Guild.Name)
                .AddField("Created", $"{cAt.Month}.{cAt.Day}.{cAt.Year} ({cAt.Humanize()})")
                .AddField("Region", Context.Guild.VoiceRegionId)
                .AddField("Members", Context.Guild.Users.Count, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Voice Channels", Context.Guild.VoiceChannels.Count, true)
                .AddField("Text Channels", Context.Guild.TextChannels.Count, true));
        }

    }
}