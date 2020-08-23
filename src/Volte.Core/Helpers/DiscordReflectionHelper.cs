using System;
using DSharpPlus.Entities;

namespace Volte.Core.Helpers
{
    public static class DiscordReflectionHelper
    {
        public static readonly Func<DiscordGuild, ulong> GetOwnerId = ExpressionHelper.MemberInstance<DiscordGuild, ulong>("OwnerId");
    }
}