using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("eval {String}")]
        [RequireBotOwner]
        public Task<ActionResult> EvalAsync([Remainder] string code)
            => Ok(async () => await EvalHelper.EvaluateAsync(Context, code), false);
    }
}