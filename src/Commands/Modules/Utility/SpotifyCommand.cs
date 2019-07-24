using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("Usage: |prefix|spotify [user]")]
        public Task<ActionResult> SpotifyAsync(SocketGuildUser target = null)
        {
            var user = target ?? Context.User;
            if (user.Activity is SpotifyGame spotify)
            {
                return Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithDescription($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})\n" +
                                     $"**Album:** {spotify.AlbumTitle}\n" +
                                     $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "No duration provided.")}\n" +
                                     $"**Artists:** {spotify.Artists.Join(", ")}")
                    .WithThumbnailUrl(spotify.AlbumArtUrl));
            }

            return BadRequest("Target user isn't listening to Spotify!");
        }
    }
}