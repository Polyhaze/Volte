using Starscript;
using Starscript.Internal;

namespace Volte.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Math", "Maths")]
    [Description("Evaluate a mathematical expression.")]
    public Task<ActionResult> MathAsync(
        [Remainder, Description("The expression.")]
        string expression)
    {
        try
        {
            var result = VolteStarscript.RunMath(expression);

            return Ok(Context.CreateEmbedBuilder()
                .AddField("Input", Format.Code(expression))
                .AddField("Output", Format.Code(result.ToString())));
        }
        catch (ParseException pe)
        {
            return BadRequest($"Syntax error: {pe.Message}");
        }
        catch (StarscriptException se)
        {
            return BadRequest(se.Message);
        }
    }
}