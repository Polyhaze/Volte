using System;
using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using ICommandContext = Qmmands.ICommandContext;

namespace Volte.Commands.Checks
{
    public sealed class RequireGuildAdminAttribute : CheckBaseAttribute
    {
        public override async Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            var ctx = context.Cast<VolteContext>();
            if (ctx.User.IsAdmin(provider.Cast<ServiceProvider>())) return CheckResult.Successful;

            await ctx.ReactFailureAsync();
            return CheckResult.Unsuccessful("Insufficient permission.");
        }
    }
}