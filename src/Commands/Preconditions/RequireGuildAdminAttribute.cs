using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Preconditions
{
    public sealed class RequireGuildAdminAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsAdmin()) return CheckResult.Successful;

            await ctx.ReactFailureAsync();
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}