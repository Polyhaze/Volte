using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Eval", "Evaluate")]
        [Description("Evaluates C# code.")]
        public Task<ActionResult> EvalAsync([Remainder, Description("The C# code to execute. Can be in a codeblock with C# highlighting if you want.")] string code)
            => Ok(async () => await EvalHelper.EvaluateAsync(Context, code), false);
    }
}