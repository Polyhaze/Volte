using System;
using Discord;
using Volte.Commands;

namespace Volte.Data.Models.EventArgs
{
    public class ModActionEventArgs
    {
        public IGuildUser Moderator { get; }
        public VolteContext Context { get; }
        public ModActionType ActionType { get; }
        public string Reason { get; }
        public ulong? TargetId { get; }
        public IUser TargetUser { get; }
        public int? Count { get; }
        public DateTimeOffset Time { get; }
        public IGuild Guild { get; }

        public ModActionEventArgs(VolteContext ctx, ModActionType type)
        {
            Moderator = ctx.User;
            Context = ctx;
            ActionType = type;
            Time = DateTimeOffset.UtcNow;
            Guild = ctx.Guild;
        }

        public ModActionEventArgs(VolteContext ctx, ModActionType type, IUser target, string reason)
        {
            Moderator = ctx.User;
            Context = ctx;
            ActionType = type;
            Reason = reason;
            TargetUser = target;
            TargetId = TargetUser.Id;
            Time = DateTimeOffset.UtcNow;
            Guild = ctx.Guild;
        }

        public ModActionEventArgs(VolteContext ctx, ModActionType type, ulong target, string reason)
        {
            Moderator = ctx.User;
            Context = ctx;
            ActionType = type;
            TargetId = target;
            Reason = reason;
            Time = DateTimeOffset.UtcNow;
            Guild = ctx.Guild;
        }

        public ModActionEventArgs(VolteContext ctx, ModActionType type, ulong target)
        {
            Moderator = ctx.User;
            Context = ctx;
            ActionType = type;
            TargetId = target;
            Time = DateTimeOffset.UtcNow;
            Guild = ctx.Guild;
        }

        public ModActionEventArgs(VolteContext ctx, ModActionType type, int count)
        {
            Moderator = ctx.User;
            Context = ctx;
            ActionType = type;
            Count = count;
            Time = DateTimeOffset.UtcNow;
            Guild = ctx.Guild;
        }
    }
}