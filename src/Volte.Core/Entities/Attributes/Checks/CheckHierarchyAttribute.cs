using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var ctx = context.AsVolteContext();
            return argument switch
            {
                DiscordMember member => ctx.Member.Hierarchy > member.Hierarchy
                    ? CheckResult.Successful
                    : CheckResult.Unsuccessful(
                        "Cannot ban someone in a higher, or equal, hierarchy position than yourself."),
                DiscordRole role => ctx.Member.Hierarchy > role.Position
                    ? CheckResult.Successful
                    : CheckResult.Unsuccessful(
                        "Cannot ban someone who has a role in a higher position than your highest role."),
                _ => throw new InvalidOperationException(
                    $"You may not use the {typeof(CheckHierarchyAttribute).FullName} attribute on a parameter that is not either {typeof(DiscordRole).AsPrettyString()} or {typeof(DiscordMember).AsPrettyString()}.")
            };
        }
    }
}