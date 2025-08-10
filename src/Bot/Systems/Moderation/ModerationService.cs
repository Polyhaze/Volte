using Volte.Systems.Database;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Moderation;

public sealed class ModerationService : VolteService
{
    private readonly DatabaseService _db;

    public ModerationService(DatabaseService databaseService)
    {
        _db = databaseService;
    }

    public async Task CheckAccountAgeAsync(UserJoinedEventArgs args)
    {
        var modConfig = _db.GetData(args.Guild).Settings.Moderation;
        if (args.User.IsBot || !modConfig.CheckAccountAge || !Config.EnabledFeatures.ModLog) return;

        Debug(LogSource.Volte, "Attempting to post a VerifyAge message.");

        var c = args.User.Guild.GetTextChannel(modConfig.ActionLogChannel);
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

        var c = args.Guild.GetTextChannel(args.GuildData.Settings.Moderation.ActionLogChannel);
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
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
                    args.MessageBuilder()
                        .Action()
                        .Moderator()
                        .Case()
                        .Target(false)
                        .Reason()
                        .Time()
                ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                break;
            }
            
            case ModActionType.Unban:
            {
                IncrementAndSave(args.GuildData);
                await e.WithDescription(
                    args.MessageBuilder()
                        .Action()
                        .Moderator()
                        .Case()
                        .Target(false)
                        .Reason()
                        .Time()
                ).SendToAsync(c);
                Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Unban)}");
                break;
            }

            case ModActionType.Verify:
                await e.WithDescription(
                    args.MessageBuilder()
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
    }

    private void IncrementAndSave(GuildDataV2 gd)
    {
        gd.Moderation.CurrentModActionCase++;
        _db.Save(gd);
    }

    public readonly struct ModLogMessageBuilder(ModActionEventArgs args)
    {
        private readonly StringBuilder _sb = new();

        public ModLogMessageBuilder Reason() 
            => Append(nameof(Reason), Format.Code(string.IsNullOrEmpty(args.Reason) ? "None provided" : args.Reason));
        public ModLogMessageBuilder Action() => Append(nameof(Action), args.ActionType);

        public ModLogMessageBuilder Moderator() =>
            Append(nameof(Moderator), $"{args.Moderator.Username} ({args.Moderator.Id})");

        public ModLogMessageBuilder Channel() => Append(nameof(Channel), $"<#{args.Channel.Id}>");
        public ModLogMessageBuilder Case() => Append(nameof(Case), args.GuildData.Moderation.CurrentModActionCase);
        public ModLogMessageBuilder MessagesCleared() => Append("Messages Cleared", args.Count);

        public ModLogMessageBuilder Target(bool isOnMessageDelete)
            => isOnMessageDelete
                ? Append("Message Deleted", args.TargetId)
                : Append("User", $"{args.TargetUser} ({args.TargetUser.Id})");

        public ModLogMessageBuilder Time()
            => Append(nameof(Time), args.Time.ToDiscordTimestamp(TimestampType.LongDateTime));

        private ModLogMessageBuilder Append(string part, object content)
            => this.Apply(it => it._sb.AppendLine($"{Format.Bold($"{part}:")} {content}"));

        public static implicit operator string(ModLogMessageBuilder messageBuilder) => messageBuilder._sb.ToString();
    }
}