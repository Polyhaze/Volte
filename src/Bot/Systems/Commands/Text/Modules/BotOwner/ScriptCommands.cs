namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class BotOwnerModule
{
    private const string EvalEnvSource = "https://github.com/Polyhaze/Volte/blob/v4/src/Bot/Entities/EvalEnvironment.cs";
    
    [Command("Evaluate", "Eval", "Repl")]
    [Description($"Evaluates C# code. You have implicit access to the methods defined in [EvalEnvironment]({EvalEnvSource}).")]
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
    [Description("Shows the inheritance tree of a .NET type. This just runs an eval using the Inheritance<T>() function.")]
    public Task<ActionResult> InheritanceAsync(
        [Remainder, Description("The .NET type to show inheritance for; aka the value of T.")]
        string type)
        => EvalAsync($"Inheritance<{type}>()");
}