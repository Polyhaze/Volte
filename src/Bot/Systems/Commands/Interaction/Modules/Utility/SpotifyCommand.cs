using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionUtilityModule
{
    [SlashCommand("spotify", "Shows what you're listening to on Spotify, if you're listening to something.")]
    public Task<RuntimeResult> SpotifyAsync(
        [Summary(description: "The member whose Spotify you want to see. Defaults to yourself.")]
        SocketUser user)
    {
        user ??= Context.User;

        return user.TryGetSpotifyStatus(out var spotify)
            ? Ok(Context.CreateEmbedBuilder()
                    .WithAuthor(user)
                    .WithDescription(sb =>
                        sb.AppendLine($"**Track:** {Format.Url(spotify.TrackTitle, spotify.TrackUrl)}")
                            .AppendLine($"**Album:** {spotify.AlbumTitle}")
                            .AppendLine(
                                $"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "<not provided>")}")
                            .AppendLine($"**Artist(s):** {spotify.Artists.JoinToString(", ")}")
                            .AppendLine(
                                $"**Started At:** {spotify.StartedAt?.ToDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}")
                            .AppendLine(
                                $"**Ends At:** {spotify.EndsAt?.ToDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}"))
                    .WithThumbnailUrl(spotify.AlbumArtUrl),
                ephemeral: true
            )
            : BadRequest("Target user isn't listening to Spotify!");
    }
}