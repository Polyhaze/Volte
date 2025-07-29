using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction;

public class SelfRoleAssignAutocompleter : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context, 
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, 
        IServiceProvider services)
    {
        if (context.Guild is not SocketGuild guild) 
            return AutocompletionResult.FromSuccess();
        
        var guildData = services.Get<DatabaseService>().GetData(context.Guild);
        
        foreach (var option in autocompleteInteraction.Data.Options)
        {
            var userValue = option.Value?.ToString();

            if (!option.Focused || string.Empty.Equals(userValue) || userValue == null) continue;
            
            var selfRoles = guild.Roles.Where(x => guildData.Extras.SelfRoles.Contains(x.Id));

            var validSelfRoles = selfRoles.Except(guild.GetUser(context.User.Id).Roles).ToList();

            if (validSelfRoles.Count > 0)
            {
                return AutocompletionResult.FromSuccess(validSelfRoles
                    .Select(x => new AutocompleteResult(x.Name, x.Id)));
            }
        }

        return AutocompletionResult.FromSuccess();
    }
}