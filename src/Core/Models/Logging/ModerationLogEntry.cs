using System;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;

namespace BrackeysBot
{
    public struct ModerationLogEntry
    {
        public ICommandContext Context { get; set; }
        public IUser Moderator { get; set; }
        public IUser Target { get; set; }
        public ulong TargetID { get; set; }
        public ITextChannel Channel { get; set; }
        public ModerationActionType ActionType { get; set; }
        public string Reason { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTimeOffset Time { get; set; }
        public string AdditionalInfo { get; set; }
        public static ModerationLogEntry New
            => new ModerationLogEntry();

        public bool HasTarget => Target != null || TargetID != 0;
        public string TargetMention => Target != null
            ? Target.Mention
            : $"<@{TargetID}>";

        public ModerationLogEntry WithContext(ICommandContext context)
        {
            Context = context;
            return this;
        }
        public ModerationLogEntry WithModerator(IUser moderator)
        {
            Moderator = moderator;
            return this;
        }
        public ModerationLogEntry WithChannel(ITextChannel channel)
        {
            Channel = channel;
            return this;
        }
        public ModerationLogEntry WithActionType(ModerationActionType actionType)
        {
            ActionType = actionType;
            return this;
        }
        public ModerationLogEntry WithTarget(IUser target)
        {
            Target = target;
            TargetID = target.Id;
            return this;
        }
        public ModerationLogEntry WithTarget(ulong id)
        {
            TargetID = id;
            return this;
        }
        public ModerationLogEntry WithReason(string reason)
        {
            Reason = reason;
            return this;
        }
        public ModerationLogEntry WithDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }
        public ModerationLogEntry WithTime(DateTimeOffset time)
        {
            Time = time;
            return this;
        }

        public ModerationLogEntry WithAdditionalInfo(string info) 
        {
            AdditionalInfo = info;
            return this;
        }
        
        public ModerationLogEntry WithDefaultsFromContext(ICommandContext context)
        {
            return WithContext(context)
                .WithModerator(context.User)
                .WithTime(DateTimeOffset.Now);
        }
    }
}
