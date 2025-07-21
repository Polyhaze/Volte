using Starscript;
using Starscript.Internal;

namespace Volte.Helpers;

public static class VolteStarscript
{
    public static readonly StarscriptHypervisor Hypervisor = StarscriptHypervisor.CreateWithStdLib();

    public static readonly StarscriptHypervisor MathHypervisor = StarscriptHypervisor.CreateStandalone().WithStandardLibraryMath();

    
    public static StringSegment Run(string source, ValueMap environment)
    {
        var script = Compile(source);

        var hv = Hypervisor.CopyGlobalsToNew();

        foreach (var entry in environment)
        {
            hv.Set(entry.Key, entry.Value);
        }

        return hv.Run(script).Apply(_ => hv.Clear());
    }

    public static StringSegment Run<T>(string source, IStarscriptObject<T> environment) where T : IStarscriptObject<T>
        => Run(source, environment.ToStarscript());

    public static StringSegment RunMath(string expression)
        => CompileExpression(expression).Execute(MathHypervisor);
    
    public static Script Compile(string source)
    {
        if (!Parser.TryParse(source, out var result))
            throw new ParseException(result.Errors.First());

        return Compiler.SingleCompile(result);
    }

    public static Script CompileExpression(string source) => Compile($"{{{source}}}");
}