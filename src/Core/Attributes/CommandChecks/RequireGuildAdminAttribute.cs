using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Attributes
{
    public sealed class RequireGuildAdminAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsAdmin(ctx)) return CheckResult.Successful;
            
            return CheckResult.Failed("Insufficient permission.");
        }
    }
}