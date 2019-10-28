using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        public EvalService Eval { get; set; }

        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("eval {code}")]
        [RequireBotOwner]
        public Task<ActionResult> EvalAsync([Remainder] string code)
            => None(() => Eval.EvaluateAsync(Context, code), false);
    }
}