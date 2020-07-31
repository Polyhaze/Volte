using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequireGuruAttribute : RequirePermissionLevelAttribute
    {
        public RequireGuruAttribute() : base(PermissionLevel.Guru) { }
    }
}
