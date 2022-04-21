using System;
using Discord.WebSocket;
using Volte.Commands;
using Volte.Interactions;

namespace Volte.Entities
{
    public class ModActionEventArgs
    {
        public SocketGuildUser Moderator { get; private set; }
        public ModActionType ActionType { get; private set; }
        public string Reason { get; private set; }
        public ulong? TargetId { get; private set; }
        public SocketUser TargetUser { get; private set; }
        public int? Count { get; private set; }
        public DateTimeOffset Time { get; private set; }
        public SocketGuild Guild { get; private set; }

        public static ModActionEventArgs New => new ModActionEventArgs();

        public ModActionEventArgs WithModerator(SocketGuildUser user)
        {
            Moderator = user;
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

        public ModActionEventArgs WithTarget(SocketUser user)
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

        public ModActionEventArgs WithGuild(SocketGuild guild)
        {
            Guild = guild;
            return this;
        }

        public ModActionEventArgs WithDefaultsFromContext(VolteContext ctx)
        {
            return WithTime(DateTime.Now)
                .WithGuild(ctx.Guild)
                .WithModerator(ctx.User);
        }
        
        public ModActionEventArgs WithDefaultsFromContext<T>(InteractionContext<T> ctx) where T : SocketInteraction
        {
            return WithTime(DateTime.Now)
                .WithGuild(ctx.Guild)
                .WithModerator(ctx.GuildUser);
        }
    }
}