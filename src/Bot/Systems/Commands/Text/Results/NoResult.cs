namespace Volte.Systems.Commands.Text;

public class NoResult : ActionResult
{
    public NoResult(AsyncFunction afterCompletion = null, bool awaitCallback = true)
    {
        _after = afterCompletion;
        _awaitCallback = awaitCallback;
    }

    private readonly AsyncFunction _after;
    private readonly bool _awaitCallback;

    public override async ValueTask<Gommon.Optional<ResultCompletionData>> ExecuteResultAsync(VolteContext ctx)
    {
        if (_after is null)
            return default;

        if (_awaitCallback)
            await _after();
        else
            _ = _after();
        return default;
    }
}