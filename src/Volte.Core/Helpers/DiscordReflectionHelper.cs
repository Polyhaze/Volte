using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace Volte.Core.Helpers
{
    public static class DiscordReflectionHelper
    {
        public static readonly Func<DiscordGuild, ulong> GetOwnerId = ExpressionHelper.GetMemberInstance<DiscordGuild, ulong>("OwnerId");

        public sealed class DiscordEmojiBuilder
        {
            private static readonly Func<DiscordEmoji> NewDiscordEmoji = ExpressionHelper.Constructor0Args<DiscordEmoji>();
            
            private static readonly Func<DiscordEmoji, List<ulong>> Roles = ExpressionHelper.GetMemberInstance<DiscordEmoji, List<ulong>>("_roles");
            private static readonly Action<DiscordEmoji, string> Name = ExpressionHelper.SetMemberInstance<DiscordEmoji, string>(nameof(Name));
            private static readonly Action<DiscordEmoji, bool> RequiresColons = ExpressionHelper.SetMemberInstance<DiscordEmoji, bool>(nameof(RequiresColons));
            private static readonly Action<DiscordEmoji, bool> IsManaged = ExpressionHelper.SetMemberInstance<DiscordEmoji, bool>(nameof(IsManaged));
            private static readonly Action<DiscordEmoji, bool> IsAnimated = ExpressionHelper.SetMemberInstance<DiscordEmoji, bool>(nameof(IsAnimated));
            
            private static readonly Action<DiscordEmoji, ulong> Id = ExpressionHelper.SetMemberInstance<DiscordEmoji, ulong>(nameof(Id));
            
            private readonly DiscordEmoji _emoji;

            public DiscordEmojiBuilder()
            {
                _emoji = NewDiscordEmoji();
            }

            public DiscordEmojiBuilder WithRoles(IEnumerable<ulong> roles)
            {
                var rolesField = Roles(_emoji);
                rolesField.Clear();
                rolesField.AddRange(roles);
                return this;
            }
            
            public DiscordEmojiBuilder WithName(string name)
            {
                Name(_emoji, name);
                return this;
            }
            public DiscordEmojiBuilder WithRequiresColons(bool requiresColons)
            {
                RequiresColons(_emoji, requiresColons);
                return this;
            }
            public DiscordEmojiBuilder WithIsManaged(bool isManaged)
            {
                IsManaged(_emoji, isManaged);
                return this;
            }
            public DiscordEmojiBuilder WithIsAnimated(bool isAnimated)
            {
                IsAnimated(_emoji, isAnimated);
                return this;
            }
            
            public DiscordEmojiBuilder WithId(ulong id)
            {
                Id(_emoji, id);
                return this;
            }

            public DiscordEmoji Build()
            {
                return _emoji;
            }
        }
    }
}