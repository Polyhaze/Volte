using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    public sealed class ModLogService : VolteEventService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public ModLogService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }

        public override Task DoAsync(EventArgs args)
            => OnModActionCompleteAsync(args.Cast<ModActionEventArgs>() ?? throw new InvalidOperationException($"ModLog was triggered with a null event. Expected: {nameof(ModActionEventArgs)}, Received: {args.GetType().Name}"));

        private async Task OnModActionCompleteAsync(ModActionEventArgs args)
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                            .AppendLine(Time(args))
                            .ToString())
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
                : $"**User:** {args.TargetUser} ({args.TargetId})";

        private string Time(ModActionEventArgs args) =>
            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}";
    }
}