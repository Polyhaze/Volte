using Discord.Interactions;

namespace Volte.Interactions.Results;

public class InteractionBadRequestResult : RuntimeResult
{
    public InteractionBadRequestResult(string error) : base(null, error) {}

    public static implicit operator Task<RuntimeResult>(InteractionBadRequestResult input) 
        => Task.FromResult<RuntimeResult>(input);
}