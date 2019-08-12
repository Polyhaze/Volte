using System.Threading.Tasks;
using Qmmands;

namespace Volte.Commands.Results
{
    public abstract class ActionResult : CommandResult
    {
        public override bool IsSuccessful { get; } = true;

        public abstract ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx);

        public static implicit operator Task<ActionResult>(ActionResult res) 
            => Task.FromResult(res);
    }
}