using System.Threading.Tasks;
using Gommon;
using Qmmands;

namespace Volte.Commands.Checks
{
    public sealed class RequireGuildAdminAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();
            if (context.AsVolteContext().Member.IsAdmin(ctx)) 
                return CheckResult.Successful;
            
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}