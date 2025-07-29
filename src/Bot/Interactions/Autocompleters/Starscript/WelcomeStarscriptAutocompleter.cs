using Discord.Interactions;
using Starscript;

namespace Volte.Interactions;

public class WelcomeStarscriptAutocompleter : AbstractStarscriptAutocompleter
{
    public override StarscriptHypervisor Hypervisor { get; protected set; }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context, 
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, 
        IServiceProvider services)
    {
        foreach (var option in autocompleteInteraction.Data.Options)
        {
            var userValue = option.Value?.ToString();

            if (!option.Focused || string.Empty.Equals(userValue) || userValue == null) continue;

            var g = services.Get<DiscordSocketClient>().GetGuild(context.Guild?.Id ?? 0);

            Hypervisor ??= VolteStarscript.Hypervisor
                .CopyGlobalsToNew()
                .ReplaceLocals(await StarscriptHelper.WrapAsync(g, context.User));
            
            var results = GetCompletions(userValue);

            if (results.Count > 0)
            {
                var autocompleteResults = results.Take(24)
                    .Select(x => new AutocompleteResult(x, userValue));
                
                return AutocompletionResult.FromSuccess(autocompleteResults
                    .Prepend(
                        new AutocompleteResult("Do not click these! Simply use this as a visual guide for how you can finish what you're typing.", userValue)));
            }
        }

        return AutocompletionResult.FromSuccess();
    }
}