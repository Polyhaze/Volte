using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    public sealed class RequireGuildModeratorAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();
            if (ctx.Member.IsModerator(ctx)) return CheckResult.Successful;
            
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}