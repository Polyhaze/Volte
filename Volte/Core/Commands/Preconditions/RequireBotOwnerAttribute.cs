using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Core.Commands.Preconditions {
    public class RequireBotOwnerAttribute : CheckBaseAttribute {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider services) {
            var ctx = (VolteContext)context;
            if (!UserUtils.IsBotOwner(ctx)) {
                await ctx.ReactFailure();
                return CheckResult.Unsuccessful("Insufficient permission.");
            }

            return CheckResult.Successful;
        }
    }
}