using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Helpers;

namespace Volte.Entities
{
    public sealed class RequireGuildAdminAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.IsAdmin(ctx.User)) return CheckResult.Successful;
            
            return CheckResult.Failed("Insufficient permission.");
        }
    }
}