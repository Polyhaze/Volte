using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Services;

namespace Volte.Commands.Modules.BotOwner
{
    public partial class BotOwnerModule : VolteModule
    {
        public EvalService Eval { get; set; }

        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        [Remarks("Usage: |prefix|eval {code}")]
        [RequireBotOwner]
        public Task<VolteCommandResult> EvalAsync([Remainder] string code)
        {
            _ = Eval.EvaluateAsync(Context, code);
            return None();
        }
    }
}