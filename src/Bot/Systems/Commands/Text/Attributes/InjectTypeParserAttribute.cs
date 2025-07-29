namespace Volte.Systems.Commands.Text;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InjectTypeParserAttribute(bool overridePrimitive = false) : Attribute
{
    public bool OverridePrimitive => overridePrimitive;
}