using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Results
{
    public abstract class ActionResult : CommandResult
    {
        public override bool IsSuccessful { get; } = true;

        public abstract Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx);

        public static implicit operator Task<ActionResult>(ActionResult res) 
            => Task.FromResult(res);
    }
}