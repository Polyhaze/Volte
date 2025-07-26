using Discord.Interactions;
using Starscript;

namespace Volte.Interactions.Commands;

public abstract class AbstractStarscriptAutocompleter : AutocompleteHandler
{
    public abstract StarscriptHypervisor Hypervisor { get; }
    
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context, 
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, 
        IServiceProvider services)
    {
        foreach (var option in autocompleteInteraction.Data.Options)
        {
            var userValue = option.Value?.ToString();

            if (!option.Focused || string.Empty.Equals(userValue) || userValue == null) continue;

            var results = new List<string>();
            
            Hypervisor.GetCompletions(userValue, userValue.Length,
                (completion, isFunction) => results.Add($"{completion}{(isFunction ? "(" : string.Empty)}"));

            if (results.Count > 0)
            {
                var autocompleteResults = results.Take(24)
                    .Select(x => new AutocompleteResult(x, userValue));
                
                return Task.FromResult(AutocompletionResult.FromSuccess(autocompleteResults
                    .Prepend(
                        new AutocompleteResult("Do not click these! Simply use this as a visual guide for how you can finish what you're typing.", userValue))));
            }
        }

        return Task.FromResult(AutocompletionResult.FromSuccess());
    }
}