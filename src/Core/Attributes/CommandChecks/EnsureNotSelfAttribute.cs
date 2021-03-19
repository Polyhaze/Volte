using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EnsureNotSelfAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<SocketGuildUser>() ?? throw new ArgumentException($"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(SocketGuildUser)}.");
            var ctx = context.Cast<VolteContext>();

            return ctx.User.Id != u.Id
                ? CheckResult.Successful
                : CheckResult.Failed("You cannot run this command on yourself.");
        }
    }
}