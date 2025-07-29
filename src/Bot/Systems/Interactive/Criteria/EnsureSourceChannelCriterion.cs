namespace Volte.Systems.Interactive;

public class EnsureSourceChannelCriterion : ICriterion<IMessage>
{
    public ValueTask<bool> JudgeAsync(VolteContext sourceContext, IMessage parameter) 
        => new ValueTask<bool>(sourceContext.Channel.Id == parameter.Channel.Id);

}