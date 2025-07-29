using Discord.Interactions;
using RunMode = Qmmands.RunMode;

namespace Volte.Systems.Interactive;

public interface IButtonCallback
{
    VolteContext MessageContext { get; }
    IUserMessage PagerMessage { get; }
    RunMode RunMode { get; }
    ICriterion<SocketInteractionContext<SocketMessageComponent>> Criterion { get; }

    ValueTask<bool> HandleAsync(SocketInteractionContext<SocketMessageComponent> buttonContext);
}