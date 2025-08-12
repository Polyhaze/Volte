using Starscript;

namespace Volte.Systems.Starscript;

public static class VolteStarscript
{
    public static bool GetBooleanValue(this StringSegment segment) 
        => bool.Parse(segment.ToString());

    public static readonly StarscriptHypervisor Hypervisor = StarscriptHypervisor.Create().WithStandardLibrary();
    
    public static readonly StarscriptHypervisor UnsafeHypervisor = StarscriptHypervisor.Create().WithStandardLibrary(@unsafe: true).WithStandardLibraryHttp();

    public static readonly StarscriptHypervisor MathHypervisor = StarscriptHypervisor.Create().WithStandardLibraryMath();

    
    public static StringSegment Run(this IGuild currentGuild, string source, IStarscriptObject environment) 
        => currentGuild.Run(source, environment.ToStarscript());

    public static StringSegment Run(Script script, IStarscriptObject environment) 
        => Run(script, environment.ToStarscript());

    public static StringSegment Run(this IGuild currentGuild, string source, ValueMap environment) 
        => Run(Compile(source, currentGuild), environment);
    
    public static StringSegment Run(Script script, ValueMap environment) 
        => Hypervisor.ReplaceLocals(environment).Run(script, environment);

    public static StringSegment RunExpression(string expression, IGuild guild)
        => CompileExpression(expression, guild).Execute(MathHypervisor);
    
    public static Script Compile(string source, IGuild guild)
    {
        if (!Parser.TryParse(source, out var result))
            throw new ParseException(result.Errors.First());

        var script = Compiler.SingleCompile(result);
        
        if (script.Code.Length > 150)
        {
            Warn(LogSource.Service, 
                $"Large compiled Starscript (>150 bytes): {{ Guild: \"{guild.Name}\" ({guild.Id})," +
                $"CodeLength: {script.Code.Length}, ConstantsCount: {script.Constants.Length} }}");
            
            if (script.Code.Length > 500)
                Warn(LogSource.Service, "It's over 500 bytes, should probably take a look!");
        }

        return script;
    }

    public static Script CompileExpression(string source, IGuild guild) => Compile($"{{{source}}}", guild);
}