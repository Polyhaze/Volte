using System;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Commands.Preconditions {
    public class RequireGuildModeratorAttribute : PreconditionAttribute {
        public override async Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services) {
            var ctx = (VolteContext)context;
            if (!UserUtils.IsModerator(ctx)) {
                await ctx.ReactFailure();
                return PreconditionResult.FromError("Insufficient permission.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}