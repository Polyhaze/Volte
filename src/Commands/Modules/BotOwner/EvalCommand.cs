using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {

        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("eval {String}")]
        [RequireBotOwner]
        public Task<ActionResult> EvalAsync([Remainder] string code)
            => None(() => Eval.EvaluateAsync(Context, code), false);
    }
}