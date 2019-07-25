using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        public EvalService Eval { get; set; }

        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public Task<ActionResult> EvalAsync([Remainder] string code)
            => None(() =>
            {
                _ = Eval.EvaluateAsync(Context, code);
                return Task.CompletedTask;
            });
    }
}