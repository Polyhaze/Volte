using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public class ModerationService : VolteService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public ModerationService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }

        public async Task CheckAccountAgeAsync(UserJoinedEventArgs args)
        {
            var c = args.User.Guild.GetTextChannel(_db.GetData(args.Guild).Configuration.Moderation.ModActionLogChannel);
            if (c is null) return;
            var now = DateTimeOffset.Now;
            var difference = now - args.User.CreatedAt;
            if (difference.Days <= 30)
            {
                var unit = difference.Days > 0 ? "days" : difference.Hours > 0 ? "hours" : "minutes";
                var time = difference.Days > 0 ? difference.Days : difference.Hours > 0 ? difference.Hours : difference.Minutes;
                await new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithTitle("Possible Malicious User")
                    .AddField("User", args.User.ToString(), true)
                    .AddField("Account Created",
                        $"{args.User.CreatedAt.FormatDate()}, {args.User.CreatedAt.FormatFullTime()}")
                    .WithFooter($"Account Created {time} {unit} ago.")
                    .WithThumbnailUrl("https://www.clipartmax.com/png/middle/22-226530_warning-signs-danger-png.png")
                    .SendToAsync(c);
            }
        }

        public async Task OnModActionCompleteAsync(ModActionEventArgs args)
        {
            if (!Config.EnabledFeatures.ModLog)
                return;

            _logger.Debug(LogSource.Volte, "Attempting to post a modlog message.");

            var c = args.Guild.GetTextChannel(args.Context.GuildData.Configuration.Moderation.ModActionLogChannel);
            if (c is null)
            {
                _logger.Debug(LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
                return;
            }

            var e = args.Context.CreateEmbedBuilder().WithAuthor(author: null);
            _logger.Debug(LogSource.Volte, "Received a signal to send a ModLog message.");
            var sb = new StringBuilder();

            switch (args.ActionType)
            {
                case ModActionType.Purge:
                {
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Count(args))
                            .AppendLine(Channel(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                    break;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(TargetUser(args, true, false))
                            .AppendLine(Channel(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                    break;
                }

                case ModActionType.Kick:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(args))
                            .AppendLine(TargetUser(args, false, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                    break;
                }

                case ModActionType.Warn:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(args))
                            .AppendLine(TargetUser(args, false, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                    break;
                }

                case ModActionType.ClearWarns:
                {
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(TargetUser(args, false, false))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                    break;
                }

                case ModActionType.Softban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(args))
                            .AppendLine(TargetUser(args, false, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                    break;
                }

                case ModActionType.Ban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(args))
                            .AppendLine(TargetUser(args, false, false))
                            .AppendLine(Reason(args))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                    break;
                }

                case ModActionType.IdBan:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(Action(args))
                            .AppendLine(Moderator(args))
                            .AppendLine(Case(args))
                            .AppendLine(TargetUser(args, false, true))
                            .AppendLine(Time(args)))
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                    break;
                }

                default:
                    throw new InvalidOperationException();
            }

            _logger.Debug(LogSource.Volte,
                "Sent a ModLog message or threw an exception.");
        }

        private void IncrementAndSave(VolteContext ctx)
        {
            ctx.GuildData.Extras.ModActionCaseNumber += 1;
            _db.UpdateData(ctx.GuildData);
        }

        private string Reason(ModActionEventArgs args) => $"**Reason:** {args.Reason}";
        private string Action(ModActionEventArgs args) => $"**Action:** {args.ActionType}";
        private string Moderator(ModActionEventArgs args) => $"**Moderator:** {args.Moderator} ({args.Moderator.Id})";
        private string Channel(ModActionEventArgs args) => $"**Channel:** {args.Context.Channel.Mention}";
        private string Case(ModActionEventArgs args) => $"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}";
        private string Count(ModActionEventArgs args) => $"**Messages Cleared:** {args.Count}";

        private string TargetUser(ModActionEventArgs args, bool isOnMessageDelete, bool isOnIdBan) => isOnMessageDelete
            ? $"**Message Deleted:** {args.TargetId}"
            : isOnIdBan
                ? $"**User:** {args.TargetId}"
                : $"**User:** {args.TargetUser} ({args.TargetUser.Id})";

        private string Time(ModActionEventArgs args) =>
            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}";
    }
}