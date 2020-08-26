using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<DiscordMember>() ?? throw new ArgumentException($"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(DiscordMember)}.");

            return context.AsVolteContext().Member.Hierarchy >= u.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("Cannot ban someone in a higher, or equal, hierarchy position than yourself.");
        }
    }
}