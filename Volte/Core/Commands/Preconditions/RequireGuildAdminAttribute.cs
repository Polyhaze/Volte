using System;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Commands.Preconditions {
    public class RequireGuildAdminAttribute : PreconditionAttribute {
        public override async Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services) {
            var ctx = (VolteContext)context;
            if (!UserUtils.IsAdmin(ctx)) {
                await ctx.ReactFailure();
                return PreconditionResult.FromError("Insufficient permission.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}