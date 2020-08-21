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
        public Task<ActionResult> EvalAsync([Remainder]string code)
            => None(async () => await Eval.EvaluateAsync(this, code), false);

        [Command("Inspect", "Insp")]
        [Description("Inspects a .NET object.")]
        [Remarks("inspect {String}")]
        public Task<ActionResult> InspectAsync([Remainder]string obj)
            => EvalAsync($"Inspect({obj})");

        [Command("Inheritance", "Inh")]
        [Description("Shows the inheritance tree of a .NET type.")]
        [Remarks("inheritance {String}")]
        public Task<ActionResult> InheritanceAsync(string type)
            => EvalAsync($"Inheritance<{type}>()");
    }
}