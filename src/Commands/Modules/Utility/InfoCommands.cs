using System.Diagnostics;
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
                .AddField("Channels", Context.Client.Guilds.SelectMany(x => x.Channels).Where(x => x is not SocketCategoryChannel).DistinctBy(x => x.Id).Count(),
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

            string GetRelevantActivity()
            {
                if (user.Activity is CustomStatusGame csg)
                {
                    if (csg.Emote is Emoji) //we are ignoring custom emojis because there is no guarantee that volte is in the guild where the emoji is from; which could lead to a massive embed field value
                    {
                        return $"{csg.Emote} {csg.State}";
                    }
                    return $"{csg.State}";
                }
                if (user.Activity is SpotifyGame)
                    return "Listening to Spotify";
                
                return user.Activity?.Name;
            }
            return Ok(Context.CreateEmbedBuilder()
                .WithTitle(user.ToString())
                .AddField("ID", user.Id, true)
                .AddField("Activity", GetRelevantActivity() ?? "Nothing", true)
                .AddField("Status", user.Status, true)
                .AddField("Is Bot", user.IsBot ? "Yes" : "No", true)
                .AddField("Role Hierarchy", user.Hierarchy, true)
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
                .WithTitle(Context.Guild.Name)
                .AddField("Created", $"{cAt.FormatDate()} ({cAt.Humanize()})")
                .AddField("Owner", Context.Guild.Owner)
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