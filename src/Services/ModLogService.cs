using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("ModLog", "The main Service for handling per-guild mod logs.")]
    public sealed class ModLogService
    {
        private readonly DatabaseService _db;

        public ModLogService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        public async Task OnModActionCompleteAsync(ModActionEventArgs args)
        {
            var config = _db.GetConfig(args.Guild);
            var c = await args.Guild.GetTextChannelAsync(config.ModerationOptions.ModActionLogChannel);
            if (c is null) return;
            var e = args.Context.CreateEmbedBuilder().WithAuthor(author: null);
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
                        config.ModerationOptions.ModActionCaseNumber += 1;
                        _db.UpdateConfig(config);
                        await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                                $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                                $"**Case:** {config.ModerationOptions.ModActionCaseNumber}\n" +
                                                $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                                $"**Reason:** {args.Reason}\n" +
                                                $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .SendToAsync(c);
                        return;
                    }

                case ModActionType.Warn:
                    {
                        config.ModerationOptions.ModActionCaseNumber += 1;
                        _db.UpdateConfig(config);
                        await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                                $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                                $"**Case:** {config.ModerationOptions.ModActionCaseNumber}\n" +
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
                        config.ModerationOptions.ModActionCaseNumber += 1;
                        _db.UpdateConfig(config);
                        await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                                $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                                $"**Case:** {config.ModerationOptions.ModActionCaseNumber}\n" +
                                                $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                                $"**Reason:** {args.Reason}\n" +
                                                $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .SendToAsync(c);

                        return;
                    }

                case ModActionType.Ban:
                    {
                        config.ModerationOptions.ModActionCaseNumber += 1;
                        _db.UpdateConfig(config);
                        await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                                $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                                $"**Case:** {config.ModerationOptions.ModActionCaseNumber}\n" +
                                                $"**User:** {args.TargetUser} ({args.TargetId})\n" +
                                                $"**Reason:** {args.Reason}\n" +
                                                $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .SendToAsync(c);

                        return;
                    }

                case ModActionType.IdBan:
                    {
                        config.ModerationOptions.ModActionCaseNumber += 1;
                        _db.UpdateConfig(config);
                        await e.WithDescription($"**Action:** {args.ActionType}\n" +
                                                $"**Moderator:** {args.Moderator} ({args.Moderator.Id})\n" +
                                                $"**Case:** {config.ModerationOptions.ModActionCaseNumber}\n" +
                                                $"**User:** {args.TargetId}\n" +
                                                $"**Reason:** {args.Reason}\n" +
                                                $"**Time:** {args.Time.FormatFullTime()}, {args.Time.FormatDate()}")
                            .SendToAsync(c);

                        return;
                    }
            }
        }
    }
}
