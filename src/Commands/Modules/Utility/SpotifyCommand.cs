using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Core.Helpers;
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
            if (target.TryGetSpotifyStatus(out var spotify))
            {
                return Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(target)
                    .AppendDescriptionLine($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})")
                    .AppendDescriptionLine($"**Album:** {spotify.AlbumTitle}")
                    .AppendDescriptionLine($"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "No duration provided.")}")
                    .AppendDescriptionLine($"**Artist(s):** {spotify.Artists.Join(", ")}")
                    .WithThumbnailUrl(spotify.AlbumArtUrl));
            }
            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}