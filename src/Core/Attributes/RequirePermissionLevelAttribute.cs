using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequirePermissionLevelAttribute : PreconditionAttribute
    {
        private readonly PermissionLevel _level;

        internal RequirePermissionLevelAttribute(PermissionLevel level)
        {
            _level = level;
        }

        internal bool MeetsPermissionLevel(ICommandContext context)
            => (context.User as IGuildUser).GetPermissionLevel(context) >= _level;
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(MeetsPermissionLevel(context)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError($"User does not meet the permission level {_level}."));
        }
    }
}
