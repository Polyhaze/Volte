using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequireAdministratorAttribute : RequirePermissionLevelAttribute
    {
        public RequireAdministratorAttribute() : base(PermissionLevel.Administrator) { }
    }
}
