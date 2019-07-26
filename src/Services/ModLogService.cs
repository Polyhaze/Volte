using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;

namespace Volte.Services
{
    [Service("ModLog", "The main Service for handling per-guild mod logs.")]
    public sealed class ModLogService
    {
        private readonly DatabaseService _db;
        private readonly LoggingService _logger;

        public ModLogService(DatabaseService databaseService,
            LoggingService loggingService)
        {
            _db = databaseService;
            _logger = loggingService;
        }

        public async Task OnModActionCompleteAsync(ModActionEventArgs args)
        {
            if (!Config.EnabledFeatures.ModLog)
                return;

            var data = _db.GetData(args.Guild);
            var c = args.Guild.GetTextChannel(data.Configuration.Moderation.ModActionLogChannel);
            if (c is null) return;
            var e = args.Context.CreateEmbedBuilder().WithAuthor(author: null);
            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service, "Received a signal to send a ModLog message.");
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
                    return;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Moderator:** {args.Moderator}\n" +
                                            $"**Message Deleted:** {args.TargetId}\n" +
                                            $"**Channel:** {args.Context.Channel.Mention}\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
                    return;
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
                    return;
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
                    break;
                }

                case ModActionType.ClearWarns:
                {
                    await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                            $"**Modertor:** {args.Moderator} ({args.Moderator.Id})\n" +
                                            $"**User:** {args.TargetUser} ({args.TargetUser.Id})\n" +
                                            $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .SendToAsync(c);
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

                    return;
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

                    return;
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

                    return;
                }

                default:
                    throw new InvalidOperationException();
            }

            await _logger.LogAsync(LogSeverity.Debug, LogSource.Service,
                "Sent a ModLog message or threw an exception.");
        }
    }
}