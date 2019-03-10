using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Preconditions
{
    public class RequireBotOwnerAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider services)
        {
            var ctx = (VolteContext) context;
            if (ctx.User.IsBotOwner()) return CheckResult.Successful;

            await ctx.ReactFailureAsync();
            return CheckResult.Unsuccessful("Insufficient permission.");

        }
    }
}