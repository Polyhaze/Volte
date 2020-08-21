using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Checks
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<SocketGuildUser>() ?? throw new ArgumentException($"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(SocketGuildUser)}.");
            var ctx = context.AsVolteContext();

            return ctx.User.Hierarchy >= u.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("Cannot ban someone in a higher, or equal, hierarchy position than yourself.");
        }
    }
}