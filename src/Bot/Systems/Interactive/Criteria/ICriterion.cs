namespace Volte.Systems.Interactive;

public interface ICriterion<in T>
{
    ValueTask<bool> JudgeAsync(VolteContext sourceContext, T parameter);
}