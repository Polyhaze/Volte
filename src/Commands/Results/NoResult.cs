using System.Threading.Tasks;
using Volte.Commands;

namespace Volte.Commands.Results
{
    public class NoResult : ActionResult
    {
        public override Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            return new ResultCompletionData();
        }
    }
}