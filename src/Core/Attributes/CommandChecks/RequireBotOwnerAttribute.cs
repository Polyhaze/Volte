using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Attributes
{
    public sealed class RequireBotOwnerAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider services)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsBotOwner()) return CheckResult.Successful;
            
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}