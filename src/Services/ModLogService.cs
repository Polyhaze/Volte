using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
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
            => OnModActionCompleteAsync(args.Cast<ModActionEventArgs>());

        private async Task OnModActionCompleteAsync(ModActionEventArgs args)
        {
            if (!Config.EnabledFeatures.ModLog)
                return;

            _logger.Log(LogSeverity.Debug, LogSource.Volte, "Attempting to post a modlog message.");

            var data = _db.GetData(args.Guild);
            var c = args.Guild.GetTextChannel(data.Configuration.Moderation.ModActionLogChannel);
            if (c is null)
            {
                _logger.Log(LogSeverity.Debug, LogSource.Volte, "Resulting channel was either not set or invalid; aborting.");
                return;
            }
            var e = args.Context.CreateEmbedBuilder().WithAuthor(author: null);
            _logger.Log(LogSeverity.Debug, LogSource.Service, "Received a signal to send a ModLog message.");
            switch (args.ActionType)
            {
                case ModActionType.Purge:
                {
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator}\n" +
                                            $"**Messages Cleared:** {args.Count}\n" +
                                            $"**Channel:** {args.Context.Channel.Mention}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                    break;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator}\n" +
                                            $"**Message Deleted:** {args.TargetId}\n" +
                                            $"**Channel:** {args.Context.Channel.Mention}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                    break;
                }

                case ModActionType.Kick:
                {
                    data.Extras.ModActionCaseNumber += 1;
                    _db.UpdateData(data);
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**Case:** {data.Extras.ModActionCaseNumber}\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                            $"**Reason:** {args.Reason}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                    break;
                }

                case ModActionType.Warn:
                {
                    data.Extras.ModActionCaseNumber += 1;
                    _db.UpdateData(data);
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**Case:** {data.Extras.ModActionCaseNumber}\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                            $"**Reason:** {args.Reason}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                    break;
                }

                case ModActionType.ClearWarns:
                {
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Modertor:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetUser.Id})\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                    break;
                }

                case ModActionType.Softban:
                {
                    data.Extras.ModActionCaseNumber += 1;
                    _db.UpdateData(data);
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**Case:** {data.Extras.ModActionCaseNumber}\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                            $"**Reason:** {args.Reason}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                    break;
                }

                case ModActionType.Ban:
                {
                    data.Extras.ModActionCaseNumber += 1;
                    _db.UpdateData(data);
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**Case:** {data.Extras.ModActionCaseNumber}\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                            $"**Reason:** {args.Reason}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                    break;
                }

                case ModActionType.IdBan:
                {
                    data.Extras.ModActionCaseNumber += 1;
                    _db.UpdateData(data);
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**Case:** {data.Extras.ModActionCaseNumber}\n" +
                                            $"**User:** {args.TargetId}\n" +
                                            $"**Reason:** {args.Reason}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    _logger.Log(LogSeverity.Debug, LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                    break;
                }

                default:
                    throw new InvalidOperationException();
            }

            _logger.Log(LogSeverity.Debug, LogSource.Volte,
                "Sent a ModLog message or threw an exception.");
        }
    }
}