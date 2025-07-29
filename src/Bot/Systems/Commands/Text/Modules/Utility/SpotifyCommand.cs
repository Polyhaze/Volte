namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Spotify")]
    [Description("Shows what you're listening to on Spotify, if you're listening to something.")]
    public Task<ActionResult> SpotifyAsync([Remainder, Description("The member whose Spotify you want to see. Defaults to yourself.")]
        SocketGuildUser target = null)
    {
        target ??= Context.User;
            
        return target.TryGetSpotifyStatus(out var spotify)
            ? Ok(Context.CreateEmbedBuilder()
                .WithAuthor(target)
                .AppendDescriptionLine($"**Track:** {Format.Url(spotify.TrackTitle, spotify.TrackUrl)}")
                .AppendDescriptionLine($"**Album:** {spotify.AlbumTitle}")
                .AppendDescriptionLine($"**Duration:** {(spotify.Duration.HasValue ? spotify.Duration.Value.Humanize(2) : "<not provided>")}")
                .AppendDescriptionLine($"**Artist(s):** {spotify.Artists.JoinToString(", ")}")
                .AppendDescriptionLine($"**Started At:** {spotify.StartedAt?.ToDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}")
                .AppendDescriptionLine($"**Ends At:** {spotify.EndsAt?.ToDiscordTimestamp(TimestampType.LongTime) ?? "<not provided>"}")
                .WithThumbnailUrl(spotify.AlbumArtUrl))
            : BadRequest("Target user isn't listening to Spotify!");
    }
}