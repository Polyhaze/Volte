using Discord.Interactions;

namespace Volte.Interactions.Results;

public class InteractionOkResult<TInteraction> : RuntimeResult where TInteraction : SocketInteraction
{
    public InteractionOkResult(ReplyBuilder<TInteraction> reply) : base(null, string.Empty)
    {
        Reply = reply;
    }

    public readonly ReplyBuilder<TInteraction> Reply;
    
    public static implicit operator Task<RuntimeResult>(InteractionOkResult<TInteraction> input) 
        => Task.FromResult<RuntimeResult>(input);
}

