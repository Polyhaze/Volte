using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Evaluate", "Eval", "Repl")]
        [Description("Evaluates C# code.")]
        public Task<ActionResult> EvalAsync(
            [Remainder, Description("The C# code to execute. Can be in a codeblock with C# highlighting if you want.")]
            string code)
            => Ok(async () => await EvalHelper.EvaluateAsync(Context, code), false);

        [Command("Inspect", "Insp")]
        [Description("Inspects a .NET object. This just runs an eval using the Inspect() function.")]
        public Task<ActionResult> InspectAsync([Remainder, Description("The .NET object to inspect.")]
            string obj)
            => EvalAsync($"Inspect({obj})");

        [Command("Inheritance", "Inh")]
        [Description(
            "Shows the inheritance tree of a .NET type. This just runs an eval using the Inheritance<T>() function.")]
        public Task<ActionResult> InheritanceAsync(
            [Remainder, Description("The .NET type to show inheritance of; aka the value of T.")]
            string type)
            => EvalAsync($"Inheritance<{type}>()");
    }
}