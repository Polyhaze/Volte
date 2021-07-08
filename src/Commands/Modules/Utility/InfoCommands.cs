using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Info")]
        [Description("Provides basic information about this instance of Volte.")]
        public async Task<ActionResult> InfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .AddField("Version", Version.FullVersion, true)
                .AddField("Author",
                    $"{await Context.Client.Rest.GetUserAsync(168548441939509248)}, contributors on [GitHub](https://github.com/Ultz/Volte), and members of the Ultz organization.",
                    true)
                .AddField("Language/Library", $"C# 8, Discord.Net {Version.DiscordNetVersion}", true)
                .AddField("Guilds", Context.Client.Guilds.Count, true)
                .AddField("Shards", Context.Client.Shards.Count, true)
                .AddField("Channels",
                    Context.Client.Guilds.SelectMany(x => x.Channels).Where(x => !(x is SocketCategoryChannel))
                        .DistinctBy(x => x.Id).Count(),
                    true)
                .AddField("Invite Me",
                    Format.Code(CommandHelper.FormatUsage(Context, CommandService.GetCommand("Invite"))), true)
                .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
                .AddField("Successful Commands", CommandsService.SuccessfulCommandCalls, true)
                .AddField("Failed Commands", CommandsService.FailedCommandCalls, true)
                .WithThumbnailUrl(Context.Client.CurrentUser.GetEffectiveAvatarUrl(size: 512)));

        [Command("UserInfo", "Ui")]
        [Description("Shows info for the mentioned user or yourself if none is provided.")]
        public Task<ActionResult> UserInfoAsync(
            [Remainder, Description("The user whose info you want to see. Defaults to yourself.")]
            SocketGuildUser user = null)
        {
            user ??= Context.User;
            
            string GetRelevantActivity() => user.Activities.FirstOrDefault() switch
            {
                //we are ignoring custom emojis because there is no guarantee that volte is in the guild where the emoji is from; which could lead to a massive (and ugly) embed field value
                CustomStatusGame {Emote: Emoji _} csg => $"{csg.Emote} {csg.State}",
                CustomStatusGame csg => $"{csg.State}",
                SpotifyGame _ => "Listening to Spotify",
                _ => user.Activities.FirstOrDefault()?.Name
            } ?? "Nothing";

            return Ok(Context.CreateEmbedBuilder()
                .WithTitle(user.ToString())
                .AddField("ID", user.Id, true)
                .AddField("Activity", GetRelevantActivity(), true)
                .AddField("Status", user.Status, true)
                .AddField("Is Bot", user.IsBot ? "Yes" : "No", true)
                .AddField("Role Hierarchy", user.Hierarchy, true)
                .AddField("Account Created",
                    $"{user.CreatedAt.FormatBoldString()}")
                .AddField("Joined This Guild",
                    $"{(user.JoinedAt.HasValue ? user.JoinedAt.Value.FormatBoldString() : DiscordHelper.Zws)}")
                .WithThumbnailUrl(user.GetEffectiveAvatarUrl(size: 512)));
        }

        [Command("GuildInfo", "Gi")]
        [Description("Shows some info about the current guild.")]
        public Task<ActionResult> GuildInfoAsync()
            => Ok(Context.CreateEmbedBuilder()
                .WithTitle(Context.Guild.Name)
                .AddField("Created", $"{Context.Guild.CreatedAt.FormatBoldString()}")
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