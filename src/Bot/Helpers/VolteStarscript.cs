using Starscript;
using Starscript.Internal;

namespace Volte.Helpers;

public static class VolteStarscript
{
    public static readonly StarscriptHypervisor Hypervisor = StarscriptHypervisor.CreateWithStdLib();

    public static readonly StarscriptHypervisor MathHypervisor = StarscriptHypervisor.CreateStandalone().WithStandardLibraryMath();

    public static StringSegment RunMath(string expression)
        => CompileMath(expression).Execute(MathHypervisor);
    
    public static Script Compile(string source)
    {
        if (!Parser.TryParse(source, out var result))
            throw new ParseException(result!.Errors.First());

        return Compiler.SingleCompile(result);
    }

    public static Script CompileMath(string source) => Compile($"{{{source}}}");
}