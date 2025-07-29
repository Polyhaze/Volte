using Volte.Systems.Interactive;

namespace Volte.Systems.Commands.Text.Modules;

public partial class UtilityModule
{
    [Command("Reverse" /*, "esreveR"*/)]
    [Description("Bot replies with the argument value reversed.")]
    public Task<ActionResult> ReverseAsync([Remainder, Description("What to reverse.")]
        string content)
        => Ok(Format.Code(content.Reverse()));

    [Command("Zalgo")]
    [Description("Generate Zalgo text.")]
    [ShowUnixArgumentsInHelp(VolteUnixCommand.Zalgo)]
    public Task<ActionResult> ZalgoAsync([Description("The content to Zalgo-ify.")]
        string content, [Remainder, Description("The Unix-style arguments for options.")]
        Dictionary<string, string> options)
    {
        if (options.TryGetValue("max", out _))
            return Ok(ZalgoHelper.GenerateZalgo(content, ZalgoIntensity.High,
                IncludeChars.Up | IncludeChars.Middle | IncludeChars.Down));

        var intensity = options.TryGetValue("intensity", out var result)
            ? result.ToLower() switch
            {
                "high" => ZalgoIntensity.High,
                "medium" => ZalgoIntensity.Medium,
                "med" => ZalgoIntensity.Medium,
                "low" => ZalgoIntensity.Low,
                _ => ZalgoIntensity.Low
            }
            : ZalgoIntensity.Low;

        IncludeChars includeChars = 0;
        if (options.TryGetValue("up", out _))
            includeChars |= IncludeChars.Up;
        if (options.TryGetValue("mid", out _) || options.TryGetValue("middle", out _))
            includeChars |= IncludeChars.Middle;
        if (options.TryGetValue("down", out _))
            includeChars |= IncludeChars.Down;
        if (includeChars is 0)
            return BadRequest("No up/middle/down characters were allowed.");

        var zalgo = ZalgoHelper.GenerateZalgo(content, intensity, includeChars);
        if (zalgo.Length > 2000)
            return BadRequest("The result was too large to show in a Discord message.");
            
        return options.TryGetValue("plain", out _)
            ? Ok(() =>
                Context.Channel.SendMessageAsync(zalgo, allowedMentions: AllowedMentions.None)
            )
            : Ok(zalgo);
    }

    [Command("Nato")]
    [Description(
        "Translates a string into the NATO Phonetic Alphabet. If no string is provided, then a full rundown of the NATO alphabet is shown.")]
    public Task<ActionResult> NatoAsync([Remainder, Description("The text to \"translate.\"")]
        string input = null)
    {
        if (input.IsNullOrEmpty())
            return Ok(new PaginatedMessage.Builder()
                .WithTitle("NATO Phonetic Alphabet")
                .WithPages(_nato.Select(kvp => $"**{char.ToUpper(kvp.Key)}**: {Format.Code(kvp.Value)}"))
                .SplitPages(12));

        // ReSharper disable once (Im) PossibleNullReferenceException
        // this legit cant happen because of the if statement above
        return TryCatch<ActionResult, ArgumentOutOfRangeException>(() =>
                Ok(Context.CreateEmbedBuilder()
                    .AddField("Input", Format.Code(input))
                    .AddField("Output", Format.Code(input.ToCharArray()
                        .Where(static x => !char.IsWhiteSpace(x))
                        .Select(x => GetNato(char.ToLower(x)))
                        .JoinToString(" "))
                    )),
            e
                => BadRequest($"There is not a NATO word for the character `{e.ParamName}`. Only standard English letters and numbers are valid."));
    }
}