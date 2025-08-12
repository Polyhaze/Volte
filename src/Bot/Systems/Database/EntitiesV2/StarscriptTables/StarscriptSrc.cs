using Starscript;
using Volte.Systems.Starscript;

namespace Volte.Systems.Database.EntitiesV2;

public class StarscriptSrc
{
    public StarscriptType Type { get; set; }

    public string CodeInput { get; set; }

    public Script Compile(IGuild guild) =>
        Type is StarscriptType.SingleExpression
            ? VolteStarscript.CompileExpression(CodeInput, guild)
            : VolteStarscript.Compile(CodeInput, guild);

    public override string ToString()
        => $"{{ Type: {Type}, Source: {CodeInput} }}";
}

public enum StarscriptType
{
    SingleExpression,
    FullString
}