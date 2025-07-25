using Starscript;
using Starscript.Internal;

namespace Volte.Helpers;

public static class VolteStarscript
{
    public static bool GetBooleanValue(this StringSegment segment) 
        => bool.Parse(segment.ToString());

    public static readonly StarscriptHypervisor Hypervisor = StarscriptHypervisor.Create().WithStandardLibrary();

    public static readonly StarscriptHypervisor MathHypervisor = StarscriptHypervisor.Create().WithStandardLibraryMath();

    
    public static StringSegment Run(string source, IStarscriptObject environment) 
        => Run(source, environment.ToStarscript());

    public static StringSegment Run(Script script, IStarscriptObject environment) 
        => Run(script, environment.ToStarscript());

    public static StringSegment Run(string source, ValueMap environment) 
        => Run(Compile(source), environment);
    
    public static StringSegment Run(Script script, ValueMap environment)
    {
        var hv = Hypervisor.ReplaceLocals(environment);

        return hv.Run(script, environment);
    }

    public static StringSegment RunExpression(string expression)
        => CompileExpression(expression).Execute(MathHypervisor);
    
    public static Script Compile(string source)
    {
        if (!Parser.TryParse(source, out var result))
            throw new ParseException(result.Errors.First());

        return Compiler.SingleCompile(result);
    }

    public static Script CompileExpression(string source) => Compile($"{{{source}}}");
}