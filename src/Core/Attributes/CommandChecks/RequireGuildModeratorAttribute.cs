using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Attributes
{
    public sealed class RequireGuildModeratorAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsModerator(provider)) return CheckResult.Successful;
            
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}