namespace Volte.Systems.Interactive;

public interface IReactionCallback
{
    RunMode RunMode { get; }
    ICriterion<SocketReaction> Criterion { get; }
    VolteContext Context { get; }

    ValueTask<bool> HandleAsync(SocketReaction reaction);
}