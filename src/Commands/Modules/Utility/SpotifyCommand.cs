using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("Usage: |prefix|spotify [user]")]
        public Task<ActionResult> SpotifyAsync(SocketGuildUser target = null)
        {
            var user = target ?? Context.User;
            if (user.Activity is SpotifyGame spotify)
                return Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithDescription(new StringBuilder()
                        .AppendLine("$ **Track:** [{ spotify.TrackTitle}]({ spotify.TrackUrl})")
                        .AppendLine($"**Album:** {spotify.AlbumTitle}")
                        .AppendLine(
                            $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "No duration provided.")}")
                        .AppendLine($"**Artists:** {spotify.Artists.Join(", ")}")
                        .ToString())
                    .WithThumbnailUrl(spotify.AlbumArtUrl));

            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}