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

        switch (args.ActionType)
        {
            case ModActionType.Purge:
            {
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .MessagesCleared()
                            .Channel()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                break;
            }

            case ModActionType.Delete:
            {
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Target(true)
                            .Channel()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                break;
            }

            case ModActionType.Kick:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Case()
                            .Target(false)
                            .Reason()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                break;
            }

            case ModActionType.Warn:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Case()
                            .Target(false)
                            .Reason()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                break;
            }

            case ModActionType.ClearWarns:
            {
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Target(false)
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                break;
            }

            case ModActionType.Softban:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Case()
                            .Target(false)
                            .Reason()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                break;
            }

            case ModActionType.Ban:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Case()
                            .Target(false)
                            .Reason()
                            .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                break;
            }

            case ModActionType.IdBan:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                        (
                            await new ModLogMessageBuilder(args)
                                .Action()
                                .Moderator()
                                .Case()
                                .TargetRestUser()
                        )
                        .Reason()
                        .Time()
                    ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                break;
            }

            case ModActionType.Verify:
                await e.WithDescription(
                        new ModLogMessageBuilder(args)
                            .Action()
                            .Moderator()
                            .Target(false)
                            .Time()
                    ).SendToAsync(c);
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

    private readonly struct ModLogMessageBuilder(ModActionEventArgs args)
    {
        private readonly StringBuilder _sb = new();

        public ModLogMessageBuilder Reason() => Append($"**Action:** {args.ActionType}");
        public ModLogMessageBuilder Action() => Append($"**Action:** {args.ActionType}");
        public ModLogMessageBuilder Moderator() => Append($"**Moderator:** {args.Moderator.GetEffectiveUsername()} ({args.Moderator.Id})");
        public ModLogMessageBuilder Channel() => Append($"**Channel:** <#{args.Channel.Id}>");
        public ModLogMessageBuilder Case() => Append($"**Case:** {args.GuildData.Extras.ModActionCaseNumber}");
        public ModLogMessageBuilder MessagesCleared() => Append($"**Messages Cleared:** {args.Count}");

        public ModLogMessageBuilder Target(bool isOnMessageDelete)
            => Append(isOnMessageDelete
                ? $"**Message Deleted:** {args.TargetId}"
                : $"**User:** {args.TargetUser.GetEffectiveUsername()} ({args.TargetUser.Id})");

        public ModLogMessageBuilder Time()
            => Append($"**Time:** {args.Time.ToDiscordTimestamp(TimestampType.LongDateTime)}");

        public async Task<ModLogMessageBuilder> TargetRestUser()
        {
            var u = await VolteBot.Client.Rest.GetUserAsync(args.TargetId ?? 0);
            return Append(u is null
                ? $"**User:** {args.TargetId}"
                : $"**User:** {u} ({args.TargetId})");
        }

        private ModLogMessageBuilder Append(string content)
            => this.Apply(it => it._sb.AppendLine(content));

        public static implicit operator string(ModLogMessageBuilder messageBuilder) => messageBuilder._sb.ToString();
    }
}