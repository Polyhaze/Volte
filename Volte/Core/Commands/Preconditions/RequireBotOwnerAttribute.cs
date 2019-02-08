using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Commands.Preconditions {
    public class RequireBotOwnerAttribute : PreconditionAttribute {
        public override async Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, CommandInfo command, IServiceProvider services) {
            var ctx = (VolteContext)context;
            if (!UserUtils.IsBotOwner(ctx)) {
                await ctx.ReactFailure();
                VolteBot.ServiceProvider.GetRequiredService<LoggingService>()
                    .Log(LogSeverity.Warning, "Module",
                        $"{ctx.User.Username}#{ctx.User.Discriminator} tried running the owner-only command \"{command.Name}\"");
                return PreconditionResult.FromError("Insufficient permission.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}