using Starscript;
using Volte.Systems.Starscript;

namespace Volte.Systems.Database.Entities;

public class StarscriptSrc
{
    public StarscriptType Type { get; set; }

    public string CodeInput { get; set; }

    public Script Compile() =>
        Type is StarscriptType.SingleExpression
            ? VolteStarscript.CompileExpression(CodeInput)
            : VolteStarscript.Compile(CodeInput);

    public override string ToString()
        => $"{{ Type: {Type}, Source: {CodeInput} }}";
}

public enum StarscriptType
{
    SingleExpression,
    FullString
}