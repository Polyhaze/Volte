using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Commands;
using Volte;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Services
{
    public class ModerationService : IVolteService
    {
        private readonly DatabaseService _db;
        private readonly DiscordShardedClient _client;

        public ModerationService(DatabaseService databaseService, IServiceProvider provider)
        {
            _db = databaseService;
            _client = provider.Get<DiscordShardedClient>();

            _client.UserJoined += u =>
                _db.GetData(u.Guild).Configuration.Moderation.CheckAccountAge && Config.EnabledFeatures.ModLog
                    ? CheckAccountAgeAsync(new UserJoinedEventArgs(u))
                    : Task.CompletedTask;
        }

        public async Task CheckAccountAgeAsync(UserJoinedEventArgs args)
        {
            if (args.User.IsBot) return;

            Logger.Debug(LogSource.Volte, "Attempting to post a VerifyAge message.");

            var c = args.User.Guild.GetTextChannel(_db.GetData(args.Guild).Configuration.Moderation
                .ModActionLogChannel);
            if (c is null) return;
            Logger.Debug(LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
            var diff = DateTimeOffset.Now - args.User.CreatedAt;
            if (diff.Days <= 30)
            {
                Logger.Debug(LogSource.Volte, "Account younger than 30 days; posting message.");
                var unit = diff.Days > 0 ? "day" : diff.Hours > 0 ? "hour" : "minute";
                var time = diff.Days > 0 ? diff.Days : diff.Hours > 0 ? diff.Hours : diff.Minutes;
                await new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithTitle("Possibly Malicious User")
                    .WithThumbnailUrl("https://img.greemdev.net/WWElGbcQHC/3112312312.png")
                    .AddField("User", args.User.ToString(), true)
                    .AddField("Account Created",
                        $"{args.User.CreatedAt.GetDiscordTimestamp(TimestampType.LongDateTime)}")
                    .WithFooter($"Account Created {unit.ToQuantity(time)} ago.")
                    .SendToAsync(c);
            }
        }

        public async Task OnModActionCompleteAsync(ModActionEventArgs args,
            EmbedBuilder embedBuilder, GuildData data, ITextChannel channel)
        {
            if (!Config.EnabledFeatures.ModLog) return;

            Logger.Debug(LogSource.Volte, "Attempting to post a modlog message.");

            var c = args.Guild.GetTextChannel(_db.GetData(args.Guild).Configuration.Moderation.ModActionLogChannel);
            if (c is null)
            {
                Logger.Debug(LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
                return;
            }

            var e = embedBuilder.WithAuthor(author: null).WithSuccessColor();
            Logger.Debug(LogSource.Volte, "Received a signal to send a ModLog message.");
            var sb = new StringBuilder();

            switch (args.ActionType)
            {
                case ModActionType.Purge:
                {
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(MessagesCleared(args))
                            .AppendLine(Channel(channel))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                    break;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Target(args, true))
                            .AppendLine(Channel(channel))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                    break;
                }

                case ModActionType.Kick:
                {
                    IncrementAndSave(data);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(data))
                            .AppendLine(Target(args, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                    break;
                }

                case ModActionType.Warn:
                {
                    IncrementAndSave(data);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(data))
                            .AppendLine(Target(args, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
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
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                    break;
                }

                case ModActionType.Softban:
                {
                    IncrementAndSave(data);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(data))
                            .AppendLine(Target(args, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                    break;
                }

                case ModActionType.Ban:
                {
                    IncrementAndSave(data);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(data))
                            .AppendLine(Target(args, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                    break;
                }

                case ModActionType.IdBan:
                {
                    IncrementAndSave(data);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(data))
                            .AppendLine(await TargetRestUser(args, _client.Rest))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                    break;
                }

                case ModActionType.Verify:
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Target(args, false))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    Logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Verify)}");
                    break;
                default:
                    throw new InvalidOperationException();
            }

            Logger.Debug(LogSource.Volte,
                "Sent a ModLog message or threw an exception.");
        }

        private void IncrementAndSave(GuildData data)
        {
            data.Extras.ModActionCaseNumber++;
            _db.Save(data);
        }

        private string Reason(ModActionEventArgs args) => $"**Reason:** {args.Reason}";
        private string Action(ModActionEventArgs args) => $"**Action:** {args.ActionType}";
        private string Moderator(ModActionEventArgs args) => $"**Moderator:** {args.Moderator} ({args.Moderator.Id})";
        private string Channel(ITextChannel channel) => $"**Channel:** {channel.Mention}";
        private string Case(GuildData data) => $"**Case:** {data.Extras.ModActionCaseNumber}";
        private string MessagesCleared(ModActionEventArgs args) => $"**Messages Cleared:** {args.Count}";

        private async Task<string> TargetRestUser(ModActionEventArgs args, DiscordSocketRestClient restClient)
        {
            var u = await restClient.GetUserAsync(args.TargetId ?? 0);
            return u is null
                ? $"**User:** {args.TargetId}"
                : $"**User:** {u} ({args.TargetId})";
        }

        private string Target(ModActionEventArgs args, bool isOnMessageDelete) => isOnMessageDelete
            ? $"**Message Deleted:** {args.TargetId}"
            : $"**User:** {args.TargetUser} ({args.TargetUser.Id})";

        private string Time(ModActionEventArgs args)
            => $"**Time:** {args.Time.GetDiscordTimestamp(TimestampType.LongDateTime)}";
    }
}