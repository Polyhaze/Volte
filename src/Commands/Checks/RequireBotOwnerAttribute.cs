using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gommon;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Commands.Checks
{
    public sealed class RequireBotOwnerAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider services)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsBotOwner()) return CheckResult.Successful;

            await ctx.ReactFailureAsync();
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}