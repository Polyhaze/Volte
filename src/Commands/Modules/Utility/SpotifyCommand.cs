using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("spotify [User]")]
        public Task<ActionResult> SpotifyAsync(DiscordMember target = null)
        {
            target ??= Context.Member;
            if (target.Activity is SpotifyGame spotify) // TODO DSharpPlus does not cache activity.
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
                    .WithThumbnail(spotify.AlbumArtUrl));
            }

            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}