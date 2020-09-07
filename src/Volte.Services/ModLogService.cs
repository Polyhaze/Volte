using System;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Entities;

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
            if (!Config.EnabledFeatures.ModLog) return;

            _logger.Debug(LogSource.Volte, "Attempting to post a modlog message.");
            
            var c = args.Guild.GetChannel(args.Context.GuildData.Configuration.Moderation.ModActionLogChannel);
            if (c is null) return;

            var e = args.Context.CreateEmbedBuilder().WithAuthor(args.Moderator);
            _logger.Debug(LogSource.Volte, "Received a signal to send a ModLog message.");
            var sb = new StringBuilder();

            switch (args.ActionType)
            {
                case ModActionType.Purge:
                {
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCount())
                            .AppendLine(args.FormatChannel())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Purge)}");
                    break;
                }

                case ModActionType.Delete:
                {
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatChannel())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Delete)}");
                    break;
                }

                case ModActionType.Kick:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCase())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatReason())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetUser.Id, new ModAction
                    {
                        Moderator = args.Moderator.Id,
                        Reason = args.Reason,
                        Time = args.Context.Now,
                        Type = args.ActionType
                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Kick)}");
                    break;
                }

                case ModActionType.Warn:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCase())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatReason())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetUser.Id, new ModAction
                    {
                        Moderator = args.Moderator.Id,
                        Reason = args.Reason,
                        Time = args.Context.Now,
                        Type = args.ActionType
                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                    break;
                }

                case ModActionType.ClearWarns:
                {
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);

                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetUser.Id, new ModAction 
                    {
                        Moderator = args.Moderator.Id,
                        Reason = "",
                        Time = args.Context.Now,
                        Type = args.ActionType

                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Warn)}");
                    break;
                    }

                case ModActionType.Softban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCase())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatReason())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetUser.Id, new ModAction
                    {
                        Moderator = args.Moderator.Id,
                        Reason = args.Reason,
                        Time = args.Context.Now,
                        Type = args.ActionType
                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Softban)}");
                    break;
                }

                case ModActionType.Ban:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCase())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatReason())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetUser.Id, new ModAction
                    {
                        Moderator = args.Moderator.Id,
                        Reason = args.Reason,
                        Time = args.Context.Now,
                        Type = args.ActionType
                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.Ban)}");
                    break;
                }

                case ModActionType.IdBan:
                {
                    IncrementAndSave(args.Context);
                    await e.WithDescription(sb
                            .AppendLine(args.FormatAction())
                            .AppendLine(args.FormatModerator())
                            .AppendLine(args.FormatCase())
                            .AppendLine(args.FormatTargetUser())
                            .AppendLine(args.FormatReason())
                            .AppendLine(args.FormatTime())
                            .ToString())
                        .SendToAsync(c);
                    _db.UpdateData(args.Context.GuildData.AddActionForUser(args.TargetId ?? 0, new ModAction
                    {
                        Moderator = args.Moderator.Id,
                        Reason = args.Reason,
                        Time = args.Context.Now,
                        Type = args.ActionType
                    }));
                    _logger.Debug(LogSource.Volte, $"Posted a modlog message for {nameof(ModActionType.IdBan)}");
                    break;
                }

                default:
                    throw new InvalidOperationException($"Received type for {nameof(ModActionEventArgs)}#{nameof(args.ActionType)} that was invalid. Received " + args.ActionType);
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