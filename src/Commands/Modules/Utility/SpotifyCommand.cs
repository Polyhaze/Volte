using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Spotify")]
        [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
        [Remarks("Usage: |prefix|spotify [user]")]
        public async Task SpotifyAsync(SocketGuildUser target = null)
        {
            var user = target ?? Context.User;
            if (user.Activity is SpotifyGame spotify)
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
}