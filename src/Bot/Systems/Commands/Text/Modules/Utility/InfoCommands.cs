namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Info", "Uptime")]
    [Description("Provides basic information about this instance of Volte.")]
    public async Task<ActionResult> InfoAsync()
        => Ok(Context.CreateEmbedBuilder()
            .WithTitle($"About {Context.Client.CurrentUser.Username}#{Context.Client.CurrentUser.DiscriminatorValue}")
            .AddField("Successful Command Usages", CalledCommandsInfo.Successes + MessageService.UnsavedSuccessfulCommandCalls, true)
            .AddField("Failed Command Usages", CalledCommandsInfo.Failures + MessageService.UnsavedFailedCommandCalls, true)
            .AddField("Author", $"{await Context.Client.Rest.GetUserAsync(168548441939509248)}, contributors on {Format.Url("GitHub", "https://github.com/Polyhaze/Volte")}, and members of the Polyhaze organization.")
            .AddField("Discord Application Created", (await Context.Client.GetApplicationInfoAsync()).CreatedAt.ToDiscordTimestamp(TimestampType.LongDateTime), true)
            .AddField("Uptime", Process.GetCurrentProcess().CalculateUptime(), true)
            .AddField("Language/Library", $"C# 12, Discord.Net {Version.DiscordNetVersion}")
            .AddField("Guilds", Context.Client.Guilds.Count, true)
            .AddField("Channels",
                Context.Client.Guilds.SelectMany(x => x.Channels).Where(x => x is not SocketCategoryChannel)
                    .DistinctBy(static x => x.Id).Count(),
                true)
            .WithFooter($"Version: {Version.InformationVersion}")
            .WithThumbnailUrl(Context.Client.CurrentUser.GetDisplayAvatarUrl(size: 512)));

    [Command("UserInfo", "Ui")]
    [Description("Shows info for the mentioned user or yourself if none is provided.")]
    public Task<ActionResult> UserInfoAsync(
        [Remainder, Description("The user whose info you want to see. Defaults to yourself.")]
        SocketGuildUser user = null)
    {
        user ??= Context.User;

        var activity = getRelevantActivity();

        return Ok(Context.CreateEmbedBuilder()
            .WithTitle(user.ToString())
            .AddField("ID", user.Id, true)
            .AddField("Activity", activity.IsNullOrEmpty() ? "Nothing" : activity, true)
            .AddField("Status", user.Status, true)
            .AddField("Is Bot", user.IsBot ? "Yes" : "No", true)
            .AddField("Role Hierarchy", user.Hierarchy, true)
            .AddField("Account Created",
                $"{user.CreatedAt.ToDiscordTimestamp(TimestampType.LongDateTime)}")
            .AddField("Joined This Guild",
                $"{(user.JoinedAt.HasValue ? user.JoinedAt.Value.ToDiscordTimestamp(TimestampType.LongDateTime) : DiscordHelper.Zws)}")
            .WithThumbnailUrl(user.GetEffectiveAvatarUrl(size: 512)));
        
        string getRelevantActivity() => user.Activities.FirstOrDefault() switch
        {
            //we are ignoring custom emojis because there is no guarantee that volte is in the guild where the emoji is from; which could lead to a massive (and ugly) embed field value
            CustomStatusGame {Emote: Emoji} csg => $"{csg.Emote} {csg.State}",
            CustomStatusGame csg => $"{csg.State}",
            SpotifyGame sg => $"Listening to {Format.Url(sg.TrackTitle, sg.TrackUrl)} on Spotify",
            _ => user.Activities.FirstOrDefault()?.Name
        } ?? "Nothing";
    }

    [Command("GuildInfo", "Gi")]
    [Description("Shows some info about the current guild.")]
    public Task<ActionResult> GuildInfoAsync()
        => Ok(Context.CreateEmbedBuilder()
            .WithTitle(Context.Guild.Name)
            .AddField("Created", $"{Context.Guild.CreatedAt.ToDiscordTimestamp(TimestampType.LongDateTime)}")
            .AddField("Owner", Context.Guild.Owner)
            .AddField("Region", Context.Guild.VoiceRegionId)
            .AddField("Members", Context.Guild.Users.Count, true)
            .AddField("Roles", Context.Guild.Roles.Count, true)
            .AddField("Category Channels", Context.Guild.CategoryChannels.Count, true)
            .AddField("Voice Channels", Context.Guild.VoiceChannels.Count, true)
            .AddField("Text Channels", Context.Guild.TextChannels.Count, true)
            .WithThumbnailUrl(Context.Guild.IconUrl));
}