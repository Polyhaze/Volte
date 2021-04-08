using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Commands;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        public Task<ActionResult> SpotifyAsync([Remainder, Description("The member whose Spotify you want to see.")]
            SocketGuildUser target = null)
        {
            target ??= Context.User;
            // i-
            var spotify = Context.User.Activity?.Cast<SpotifyGame>() ??
                          Context.User.Activities.FirstOrDefault(x => x is SpotifyGame).Cast<SpotifyGame>();
            if (spotify != null)
            {
                return Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(target)
                    .WithDescription(new StringBuilder()
                        .AppendLine($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})")
                        .AppendLine($"**Album:** {spotify.AlbumTitle}")
                        .AppendLine(
                            $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "No duration provided.")}")
                        .AppendLine($"**Artist(s):** {spotify.Artists.Join(", ")}")
                        .ToString())
                    .WithThumbnailUrl(spotify.AlbumArtUrl));
            }

            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}