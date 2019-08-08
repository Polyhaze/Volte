using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Core.Models.Guild;

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
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Moderator:** {args.Moderator}")
                        .AppendLine($"**Messages Cleared:** {args.Count}")
                        .AppendLine($"**Channel:** {args.Context.Channel.Mention}")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                    break;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                            .AppendLine($"**Moderator:** {args.Moderator}")
                            .AppendLine($"**Message Deleted:** {args.TargetId}")
                            .AppendLine($"**Channel:** {args.Context.Channel.Mention}")
                            .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                    break;
                }

                case ModActionType.Kick:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                            .AppendLine($"**Moderator:** {args.Moderator} ({args.Moderator.Id})")
                            .AppendLine($"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}")
                            .AppendLine($"**User:** {args.TargetUser} ({args.TargetId})")
                            .AppendLine($"**Reason:** {args.Reason}")
                            .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                    break;
                }

                case ModActionType.Warn:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Moderator:** {args.Moderator} ({args.Moderator.Id})")
                        .AppendLine($"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}")
                        .AppendLine($"**User:** {args.TargetUser} ({args.TargetId})")
                        .AppendLine($"**Reason:** {args.Reason}")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                    break;
                }

                case ModActionType.ClearWarns:
                {
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Modertor:** {args.Moderator} ({args.Moderator.Id})")
                        .AppendLine($"**User:** {args.TargetUser} ({args.TargetUser.Id})")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.ClearWarns)}");
                    break;
                }

                case ModActionType.Softban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Moderator:** {args.Moderator} ({args.Moderator.Id})")
                        .AppendLine($"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}")
                        .AppendLine($"**User:** {args.TargetUser} ({args.TargetId})")
                        .AppendLine($"**Reason:** {args.Reason}")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                    break;
                }

                case ModActionType.Ban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb.AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Moderator:** {args.Moderator} ({args.Moderator.Id})")
                        .AppendLine($"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}")
                        .AppendLine($"**User:** {args.TargetUser} ({args.TargetId})")
                        .AppendLine($"**Reason:** {args.Reason}")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                        .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                    break;
                }

                case ModActionType.IdBan:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb.AppendLine()
                        .AppendLine($"**Action:** {args.ActionType}")
                        .AppendLine($"**Moderator:** {args.Moderator} ({args.Moderator.Id})")
                        .AppendLine($"**Case:** {args.Context.GuildData.Extras.ModActionCaseNumber}")
                        .AppendLine($"**User:** {args.TargetId}")
                        .AppendLine($"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
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
    }
}