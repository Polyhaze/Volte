namespace Volte.Systems.Interactive;

public class EmptyCriterion<T> : ICriterion<T>
{
    public ValueTask<bool> JudgeAsync(VolteContext sourceData, T parameter)
        => new(true);
}