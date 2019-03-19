/*using System.Threading.Tasks;
using DSharpPlus.Entities;
using Humanizer;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        //Removed for now. I'm not sure if D#+ has support for Spotify statuses yet.
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("Usage: |prefix|spotify [user]")]
        public async Task SpotifyAsync(DiscordMember target = null)
        {
            var user = target ?? Context.User;
            if (user.Presence.Activity)
            {
                await Context.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithDescription($"**Track:** [{spotify.TrackTitle}]({spotify.TrackUrl})\n" +
                                     $"**Album:** {spotify.AlbumTitle}\n" +
                                     $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "No duration provided.")}\n" +
                                     $"**Artists:** {string.Join(", ", spotify.Artists)}")
                    .WithThumbnailUrl(spotify.AlbumArtUrl)
                    .SendToAsync(Context.Channel);
                return;

            }
            await Context.CreateEmbed("Target user isn't listening to Spotify right now.").SendToAsync(Context.Channel);
        }
    }
}*/

