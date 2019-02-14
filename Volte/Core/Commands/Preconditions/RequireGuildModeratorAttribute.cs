using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Qmmands;
using Volte.Core.Helpers;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Core.Commands.Preconditions
{
    public class RequireGuildModeratorAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            var ctx = (VolteContext) context;
            if (!UserUtils.IsModerator(ctx))
            {
                await ctx.ReactFailure();
                return CheckResult.Unsuccessful("Insufficient permission.");
            }

            return CheckResult.Successful;
        }
    }
}