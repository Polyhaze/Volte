using System;
using DSharpPlus.Entities;
using Gommon;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public class ModActionEventArgs : EventArgs
    {
        public DiscordMember Moderator { get; private set; }
        public VolteContext Context { get; private set; }
        public ModActionType ActionType { get; private set; }
        public string Reason { get; private set; }
        public ulong? TargetId { get; private set; }
        public DiscordUser TargetUser { get; private set; }
        public int? Count { get; private set; }
        public DateTimeOffset Time { get; private set; }
        public DiscordGuild Guild { get; private set; }

        public static ModActionEventArgs New => new ModActionEventArgs();

        public ModActionEventArgs WithContext(VolteContext ctx)
        {
            Context = ctx;
            return this;
        }

        public ModActionEventArgs WithModerator(DiscordMember member)
        {
            Moderator = member;
            return this;
        }

        public ModActionEventArgs WithActionType(ModActionType type)
        {
            ActionType = type;
            return this;
        }

        public ModActionEventArgs WithReason(string reason)
        {
            Reason = reason;
            return this;
        }

        public ModActionEventArgs WithTarget(ulong? id)
        {
            TargetId = id;
            return this;
        }

        public ModActionEventArgs WithTarget(DiscordUser user)
        {
            TargetUser = user;
            return this;
        }

        public ModActionEventArgs WithCount(int? count)
        {
            Count = count;
            return this;
        }

        public ModActionEventArgs WithTime(DateTimeOffset time)
        {
            Time = time;
            return this;
        }

        public ModActionEventArgs WithGuild(DiscordGuild guild)
        {
            Guild = guild;
            return this;
        }

        public ModActionEventArgs WithDefaultsFromContext(VolteContext ctx)
        {
            WithContext(ctx)
                .WithTime(ctx.Now)
                .WithGuild(ctx.Guild)
                .WithModerator(ctx.Member);


            return this;
        }

        internal string Format(FormatType type)
        {
            return type switch
            {
                FormatType.Reason => $"**Reason:** {Reason}",
                FormatType.Action => $"**Action:** {ActionType}",
                FormatType.Moderator => $"**Moderator:** {Moderator.Mention} ({Moderator.Id})",
                FormatType.Channel => $"**Channel:** {Context.Channel.Mention}",
                FormatType.Case => $"**Case:** {Context.GuildData.Extras.ModActionCaseNumber}",
                FormatType.Count => $"**Messages Cleared:** {Count}",
                FormatType.TargetUser => ActionType switch
                {
                    ModActionType.Delete => $"**Message Deleted:** {TargetId}",
                    ModActionType.IdBan => $"**User:** {TargetId}",
                    _ => $"**User:** {TargetUser} ({TargetUser.Id})"
                },
                FormatType.Time => $"**Time:** {Time.FormatFullTime()}, {Time.FormatDate()}",
                _ => throw new FormatException("FormatType was invalid.")
            };
        }

        public string FormatReason() => Format(FormatType.Reason);
        public string FormatAction() => Format(FormatType.Action);
        public string FormatModerator() => Format(FormatType.Moderator);
        public string FormatChannel() => Format(FormatType.Channel);
        public string FormatCase() => Format(FormatType.Case);
        public string FormatCount() => Format(FormatType.Count);
        public string FormatTargetUser() => Format(FormatType.TargetUser);
        public string FormatTime() => Format(FormatType.Time);
    }

    internal enum FormatType
    {
        Reason,
        Action,
        Moderator,
        Channel,
        Case,
        Count,
        TargetUser,
        Time
    }
}