using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction;

public abstract class InteractionResultBase(InteractionCommandError? error, string reason) : RuntimeResult(error, reason)
{
    protected InteractionResultBase() : this(null, string.Empty) { }
    
    public virtual Task ExecuteAsync() => Task.CompletedTask;
    
    public static implicit operator Task<RuntimeResult>(InteractionResultBase input) 
        => Task.FromResult<RuntimeResult>(input);
}