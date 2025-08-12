using Discord.Interactions;
using Starscript;
using Volte.Systems.Starscript;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionUtilityModule
{
    [SlashCommand("math", "Evaluate a mathematical expression via Starscript.")]
    public Task<RuntimeResult> MathAsync(
        [Autocomplete<MathStarscriptAutocompleter>] 
        [Summary(description: "The expression. Powered by Starscript.")]
        string expression,
        [Summary("public", "Post the result publicly.")]
        bool publicResult = false)
    {
        try
        {
            expression = expression.Replace("{", string.Empty).Replace("}", string.Empty);

            var result = VolteStarscript.RunExpression(expression, Context.Guild);

            return Ok(Context.CreateEmbedBuilder()
                    .AddField("Input", Format.Code(expression))
                    .AddField("Output", Format.Code(result.ToString())),
                ephemeral: !publicResult
            );
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