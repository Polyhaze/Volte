namespace Volte.Systems.Commands.Text;

public abstract class ActionResult : CommandResult
{
    public override bool IsSuccessful => true;

    public abstract ValueTask<Gommon.Optional<ResultCompletionData>> ExecuteResultAsync(VolteContext ctx);

    public static implicit operator Task<ActionResult>(ActionResult res) 
        => Task.FromResult(res);
}