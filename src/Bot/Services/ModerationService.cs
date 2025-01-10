namespace Volte.Services;

public sealed class ModerationService : VolteService
{
    private readonly DatabaseService _db;
    
    public ModerationService(DatabaseService databaseService)
    {
        _db = databaseService;
    }
    
    public async Task CheckAccountAgeAsync(UserJoinedEventArgs args)
    {
        var modConfig = _db.GetData(args.Guild).Configuration.Moderation;
        if (args.User.IsBot || !modConfig.CheckAccountAge || !Config.EnabledFeatures.ModLog) return;
            
        Debug(LogSource.Volte, "Attempting to post a VerifyAge message.");
            
        var c = args.User.Guild.GetTextChannel(modConfig.ModActionLogChannel);
        if (c is null) return;
        Debug(LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
        var diff = DateTimeOffset.Now - args.User.CreatedAt;
        if (diff.Days <= 30)
        {
            Debug(LogSource.Volte, "Account younger than 30 days; posting message.");
            var unit = diff.Days > 0 ? "day" : diff.Hours > 0 ? "hour" : "minute";
            var time = diff.Days > 0 ? diff.Days : diff.Hours > 0 ? diff.Hours : diff.Minutes;
                
            await new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("Possibly Malicious User")
                .WithThumbnailUrl("https://raw.githubusercontent.com/GreemDev/VolteAssets/main/question_mark.png")
                .AddField("User", args.User.Mention, true)
                .AddField("Account Created", args.User.CreatedAt.ToDiscordTimestamp(TimestampType.LongDateTime))
                .WithFooter($"Account created {unit.ToQuantity(time)} before joining.")
                .SendToAsync(c);
        }
    }

    public async Task OnModActionCompleteAsync(ModActionEventArgs args)
    {
        if (!Config.EnabledFeatures.ModLog) return;

        Debug(LogSource.Volte, "Attempting to post a modlog message.");
        
        var c = args.Guild.GetTextChannel(args.GuildData.Configuration.Moderation.ModActionLogChannel);
        if (c is null)
        {
            Debug(LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
            return;
        }

        var e = args.CreateEmbedBuilder(null).WithAuthor(author: null).WithSuccessColor();
        Debug(LogSource.Volte, "Received a signal to send a ModLog message.");
        var sb = new StringBuilder();

        switch (args.ActionType)
        {
            case ModActionType.Purge:
            {
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(MessagesCleared(args))
                        .AppendLine(Channel(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                break;
            }

            case ModActionType.Delete:
            {
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Target(args, true))
                        .AppendLine(Channel(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                break;
            }

            case ModActionType.Kick:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Case(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Reason(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                break;
            }

            case ModActionType.Warn:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Case(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Reason(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                break;
            }

            case ModActionType.ClearWarns:
            {
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                break;
            }

            case ModActionType.Softban:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Case(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Reason(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                break;
            }

            case ModActionType.Ban:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Case(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Reason(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                break;
            }

            case ModActionType.IdBan:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Case(args))
                        .AppendLine(await TargetRestUser(args))
                        .AppendLine(Reason(args))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                break;
            }

            case ModActionType.Verify:
                await e.WithDescription(sb
                        .AppendLine(Action(args))
                        .AppendLine(Moderator(args))
                        .AppendLine(Target(args, false))
                        .AppendLine(Time(args)))
                    .SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Verify)}");
                break;
            
            default:
                Debug(LogSource.Volte, "What the hell did you pass as a ModActionType?", InvocationInfo.Here());
                break;
        }

        Debug(LogSource.Volte,
            "Sent a ModLog message or threw an exception.");
    }

    private void IncrementAndSave(GuildData gd)
    {
        gd.Extras.ModActionCaseNumber++;
        _db.Save(gd);
    }

    private static string Reason(ModActionEventArgs args) => $"**Reason:** {args.Reason}";
    private static string Action(ModActionEventArgs args) => $"**Action:** {args.ActionType}";
    private static string Moderator(ModActionEventArgs args) => $"**Moderator:** {args.Moderator} ({args.Moderator.Id})";
    private static string Channel(ModActionEventArgs args) => $"**Channel:** <#{args.Channel.Id}>";
    private static string Case(ModActionEventArgs args) => $"**Case:** {args.GuildData.Extras.ModActionCaseNumber}";
    private static string MessagesCleared(ModActionEventArgs args) => $"**Messages Cleared:** {args.Count}";

    private static async Task<string> TargetRestUser(ModActionEventArgs args)
    {
        var u = await VolteBot.Client.Rest.GetUserAsync(args.TargetId ?? 0);
        return u is null
            ? $"**User:** {args.TargetId}"
            : $"**User:** {u} ({args.TargetId})";
    }
    private static string Target(ModActionEventArgs args, bool isOnMessageDelete) => isOnMessageDelete
        ? $"**Message Deleted:** {args.TargetId}"
        : $"**User:** {args.TargetUser} ({args.TargetUser.Id})";

    private static string Time(ModActionEventArgs args)
        => $"**Time:** {args.Time.ToDiscordTimestamp(TimestampType.LongDateTime)}";
}