namespace Volte.Services;

public sealed class GuildService : VolteService
{
    private readonly DiscordSocketClient _client;

    public GuildService(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public async Task OnJoinAsync(JoinedGuildEventArgs args)
    {
        Debug(LogSource.Volte, "Joined a guild.");
        if (Config.BlacklistedOwners.Contains(args.Guild.Owner.Id))
        {
            Warn(LogSource.Volte,
                $"Left guild \"{args.Guild.Name}\" owned by blacklisted owner {args.Guild.Owner}.");
            await args.Guild.LeaveAsync();
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("Hey there!")
            .WithAuthor(await _client.Rest.GetUserAsync(Config.Owner))
            .WithColor(Config.SuccessColor)
            .WithDescription("Thanks for inviting me! Here's some basic instructions on how to set me up.")
            .AddField("Set your staff roles", "$setup", true)
            .AddField("Permissions", sb =>
                sb.AppendLine(
                        "It is recommended to give me the Administrator permission to avoid any permission errors that may happen.")
                    .AppendLine(
                        "You *can* get away with just send messages, ban members, kick members, and the like if you don't want to give me admin; however")
                    .AppendLine("if you're wondering why you're getting permission errors, that's *probably* why.")
            );

        Debug(LogSource.Volte,
            "Attempting to send the guild owner the introduction message.");
        try
        {
            await embed.SendToAsync(args.Guild.Owner);
            Error(LogSource.Volte,
                "Sent the guild owner the introduction message.");
        }
        catch (Exception)
        {
            var c = args.Guild.TextChannels.MaxBy(static x => x.Position);
            Error(LogSource.Volte,
                "Could not DM the guild owner; sending to the upper-most channel instead.");
            if (c != null) await embed.SendToAsync(c);
        }
            
    }
}