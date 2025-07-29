using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction;

public class InteractionNoneResult<TInteraction> : InteractionResultBase where TInteraction : SocketInteraction
{
    private readonly bool _didDefer;
    private readonly SocketInteractionContext<TInteraction> _context;
    
    public InteractionNoneResult(SocketInteractionContext<TInteraction> context, bool deferred)
    {
        _context = context;
        _didDefer = deferred;
    }

    public override async Task ExecuteAsync()
    {
        if (!_didDefer)
            await _context.Interaction.DeferAsync();
        
        await _context.Interaction.DeleteOriginalResponseAsync();
    }
}