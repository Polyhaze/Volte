using Starscript;
using Starscript.Internal;
using Volte.Systems.Starscript;

namespace Volte.Systems.Commands.Text.Modules;

public partial class BotOwnerModule
{
        [Command("Starscript", "Ss")]
    [Description(
        "Evaluates Starscript code. You have access to the entire Starscript.Net Standard Library, including unsafe and HTTP.")]
    public Task<ActionResult> StarscriptAsync(
        [Remainder, Description("The Starscript code to execute. Can be in a codeblock if you want.")]
        string code)
    {
        if (!Parser.TryParse(code, out ParserResult result))
            return BadRequest(String(sb =>
                    {
                        sb.AppendLine(Format.Bold("Syntax error".ToQuantity(result.Errors.Count))).AppendLine();

                        result.Errors.ForEachIndexed((e, i) => sb.AppendLine($"{Format.Code(i.ToString(), string.Empty)}: `{e}`"));
                    }
                )
            );

        var script = Compiler.SingleCompile(result);

        var hv = VolteStarscript.UnsafeHypervisor.ReplaceLocals(Context);

        try
        {
            var executionResult = script.Execute(hv).ToString();

            if (executionResult.Length + 2 > 1024)
            {
                if (executionResult.Length + 2 > EmbedBuilder.MaxDescriptionLength)
                    return Ok("Execution succeeded; but the result was too big to display in an embed.");
                
                return Ok(Context.CreateEmbedBuilder()
                    .WithDescription(Format.Code(script.Execute(hv).ToString(), string.Empty))
                    .AddField("Input", Format.Code(code, string.Empty)));
            }

            return Ok(Context.CreateEmbedBuilder()
                .AddField("Input", Format.Code(code, string.Empty))
                .AddField("Result",
                    Format.Code(script.Execute(hv).ToString(), string.Empty)));


        }
        catch (StarscriptException se)
        {
            return BadRequest($"Hypervisor error: {Format.Code(se.Message)}");
        }
        catch (Exception e)
        {
            return BadRequest($"Execution error: {Format.Code(e.Message)}");
        }
    }
}