using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequireModeratorAttribute : RequirePermissionLevelAttribute
    {
        public RequireModeratorAttribute() : base(PermissionLevel.Moderator) { }
    }
}
