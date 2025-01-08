namespace Volte.Interactions.Commands.Modules;

[Discord.Interactions.Group("mod", "Moderator-only commands.")]
[RequireGuildModeratorPrecondition]
public sealed partial class InteractionModerationModule : VolteSlashCommandModule
{
    public ModerationService ModService { get; set; }
    
    public static async Task WarnAsync(
        SocketGuildUser issuer, 
        GuildData data,
        SocketGuildUser member,
        DatabaseService db, 
        string reason)
    {
        data.Extras.Warns.Add(new Warn
        {
            User = member.Id,
            Reason = reason,
            Issuer = issuer.Id,
            Date = DateTimeOffset.Now
        });
        db.Save(data);

        var e = new EmbedBuilder().WithRelevantColor(issuer).WithAuthor(issuer)
            .WithDescription($"You've been warned in {Format.Bold(member.Guild.Name)} for {Format.Code(reason)}.")
            .Apply(data);

        if (!await member.TrySendMessageAsync(embed: e.Build()))
            Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
            
    }
}