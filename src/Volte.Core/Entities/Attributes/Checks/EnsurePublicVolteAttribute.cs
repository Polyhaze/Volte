using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EnsurePublicVolteAttribute : CheckAttribute
    {
        private readonly (ulong Developer, ulong Public, ulong Private) _validIds = (168548441939509248, 320942091049893888, 410547925597421571);

        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var ctx = context.AsVolteContext();

            var isPublicVolte = ctx.Member.Id == _validIds.Developer && 
                (ctx.Client.CurrentUser.Id == _validIds.Public || ctx.Client.CurrentUser.Id == _validIds.Private);

            if (isPublicVolte)
                return CheckResult.Successful;
            else
                return CheckResult.Unsuccessful("This command cannot be used on a non-offical version of Volte as it will not work.");

        }
    }
}
