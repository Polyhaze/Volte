using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Core.Helpers;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Core.Commands.Preconditions
{
    public class RequireBotOwnerAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider services)
        {
            var ctx = (VolteContext) context;
            if (!UserUtils.IsBotOwner(ctx))
            {
                await ctx.ReactFailureAsync();
                return CheckResult.Unsuccessful("Insufficient permission.");
            }

            return CheckResult.Successful;
        }
    }
}