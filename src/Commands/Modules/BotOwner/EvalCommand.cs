using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("eval {String}")]
        public Task<ActionResult> EvalAsync([Remainder] string code)
            => None(async () => await Eval.EvaluateAsync(this, code), false);
    }
}