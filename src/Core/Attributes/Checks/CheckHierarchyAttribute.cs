using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<SocketGuildUser>() ?? throw new ArgumentException(
                $"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(SocketGuildUser)}.");
            var ctx = context.Cast<VolteContext>();

            if (u.IsAdmin(ctx)) return CheckResult.Failed("Cannot ban someone with the configured Admin role.");
            
            return u.IsAdmin(ctx)
                ? CheckResult.Failed("Cannot ban someone with the configured Admin role.")
                : ctx.User.Hierarchy >= u.Hierarchy
                    ? CheckResult.Successful
                    : CheckResult.Failed("Cannot ban someone in a higher, or equal, hierarchy position than yourself.");
        }
    }
}