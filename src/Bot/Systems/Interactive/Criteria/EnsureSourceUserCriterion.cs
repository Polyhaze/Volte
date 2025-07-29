namespace Volte.Systems.Interactive;

public class EnsureSourceUserCriterion : ICriterion<IMessage>
{
    public ValueTask<bool> JudgeAsync(VolteContext sourceContext, IMessage parameter) 
        => new ValueTask<bool>(sourceContext.User.Id == parameter.Author.Id);

}